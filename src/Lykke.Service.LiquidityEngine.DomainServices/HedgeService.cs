using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Metrics;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public class HedgeService : IHedgeService
    {
        private const int MaxIterations = 50;

        private readonly IPositionService _positionService;
        private readonly IInstrumentService _instrumentService;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly IMarketMakerStateService _marketMakerStateService;
        private readonly IRemainingVolumeService _remainingVolumeService;
        private readonly ILog _log;
        private readonly object _syncManualResetEvent = new object();
        private readonly ManualResetEventSlim _manualResetEvent = new ManualResetEventSlim(false, 1);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Dictionary<string, Tuple<int, int>> _attempts =
            new Dictionary<string, Tuple<int, int>>();

        public HedgeService(
            IPositionService positionService,
            IInstrumentService instrumentService,
            IExternalExchangeService externalExchangeService,
            IMarketMakerStateService marketMakerStateService,
            IRemainingVolumeService remainingVolumeService,
            ILogFactory logFactory)
        {
            _positionService = positionService;
            _instrumentService = instrumentService;
            _externalExchangeService = externalExchangeService;
            _marketMakerStateService = marketMakerStateService;
            _remainingVolumeService = remainingVolumeService;
            _log = logFactory.CreateLog(this);
        }

        public void Start()
        {
            try
            {
                Task.Run(TryHedgeAsync);
            }
            catch (Exception ex)
            {
                _log.Warning("Something went wrong in HedgeService.Start().", ex);
            }
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();
        }

        public async Task TryHedgeAsync()
        {
            while (true)
            {
                try
                {
                    try
                    {
                        _manualResetEvent.Wait(_cancellationTokenSource.Token);
                        lock (_syncManualResetEvent)
                        {
                            _manualResetEvent.Reset();
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }

                    var startedAt = DateTime.UtcNow;

                    IReadOnlyCollection<Position> positions = await _positionService.GetOpenAllAsync();

                    await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Long), PositionType.Long);

                    await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Short), PositionType.Short);

                    await CloseRemainingVolumeAsync();

                    var finishedAt = DateTime.UtcNow;

                    if (positions.Count != 0)
                        _log.Info("HedgeService.ExecuteAsync() completed.", new
                        {
                            PositionsCount = positions.Count,
                            TradeIds = positions.Select(x => x.TradeId).ToList(),
                            StartedAt = startedAt,
                            FinishedAt = finishedAt,
                            Latency = (finishedAt - startedAt).TotalMilliseconds
                        });

                    PrometheusMetrics.HedgeTotalLatency.Inc((finishedAt - startedAt).TotalMilliseconds);
                }
                catch (Exception ex)
                {
                    _log.Error("Something went wrong in TryHedgeAsync.", ex);
                }
            }
        }

        public async Task ExecuteAsync()
        {
            lock (_syncManualResetEvent)
            {
                _manualResetEvent.Set();
            }
        }

        private async Task ClosePositionsAsync(IEnumerable<Position> positions, PositionType positionType)
        {
            foreach (IGrouping<string, Position> group in positions.GroupBy(o => o.AssetPairId))
            {
                var startedAt = DateTime.UtcNow;

                MarketMakerState marketMakerState = await _marketMakerStateService.GetStateAsync();

                if (marketMakerState.Status != MarketMakerStatus.Active)
                    continue;

                Instrument instrument = await _instrumentService.GetByAssetPairIdAsync(group.Key);

                decimal originalVolume = group.Sum(o => o.Volume);

                decimal volume = Math.Round(originalVolume, instrument.VolumeAccuracy);

                if (volume < instrument.MinVolume)
                {
                    _log.InfoWithDetails("The volume of open positions is less than min volume", new
                    {
                        instrument.AssetPairId,
                        positionType,
                        volume,
                        instrument.MinVolume
                    });
                    continue;
                }

                ExternalTrade externalTrade = await ExecuteLimitOrderAsync(group.Key, volume, positionType);

                if (externalTrade != null)
                {
                    await _positionService.CloseAsync(group.ToArray(), externalTrade);

                    await _remainingVolumeService.RegisterVolumeAsync(group.Key,
                        (originalVolume - externalTrade.Volume) * GetSign(positionType));
                }

                var finishedAt = DateTime.UtcNow;

                _log.Info("HedgeService.ClosePositionsAsync() completed.", new
                {
                    AssetPairId = instrument.AssetPairId,
                    TradeIds = positions.Select(x => x.TradeId).ToList(),
                    StartedAt = startedAt,
                    FinishedAt = finishedAt,
                    Latency = (finishedAt - startedAt).TotalMilliseconds
                });

                PrometheusMetrics.HedgeAssetPairLatency.Inc((finishedAt - startedAt).TotalMilliseconds);
            }
        }

        private async Task CloseRemainingVolumeAsync()
        {
            var startedAt = DateTime.UtcNow;

            IReadOnlyCollection<RemainingVolume> remainingVolumes = await _remainingVolumeService.GetAllAsync();

            foreach (RemainingVolume remainingVolume in remainingVolumes)
            {
                MarketMakerState marketMakerState = await _marketMakerStateService.GetStateAsync();

                if (marketMakerState.Status != MarketMakerStatus.Active)
                    continue;

                Instrument instrument = await _instrumentService.GetByAssetPairIdAsync(remainingVolume.AssetPairId);

                if (Math.Abs(remainingVolume.Volume) < instrument.MinVolume)
                    continue;

                decimal volume = Math.Round(Math.Abs(remainingVolume.Volume), instrument.VolumeAccuracy);

                PositionType positionType = remainingVolume.Volume > 0
                    ? PositionType.Long
                    : PositionType.Short;

                ExternalTrade externalTrade =
                    await ExecuteLimitOrderAsync(instrument.AssetPairId, volume, positionType);

                if (externalTrade != null)
                {
                    await _positionService.CloseRemainingVolumeAsync(instrument.AssetPairId, externalTrade);

                    await _remainingVolumeService.RegisterVolumeAsync(instrument.AssetPairId,
                        externalTrade.Volume * GetSign(positionType) * -1);
                }
            }

            var finishedAt = DateTime.UtcNow;

            _log.Info("HedgeService.CloseRemainingVolumeAsync() completed.", new
            {
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                Latency = (finishedAt - startedAt).TotalMilliseconds
            });
        }

        private async Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume,
            PositionType positionType)
        {
            var startedAt = DateTime.UtcNow;

            ExternalTrade externalTrade = null;

            if (!_attempts.TryGetValue(assetPairId, out Tuple<int, int> attempt))
                attempt = new Tuple<int, int>(0, 0);

            try
            {
                if (attempt.Item2 <= 0)
                {
                    if (positionType == PositionType.Long)
                        externalTrade = await _externalExchangeService.ExecuteSellLimitOrderAsync(assetPairId, volume);
                    else
                        externalTrade = await _externalExchangeService.ExecuteBuyLimitOrderAsync(assetPairId, volume);

                    _attempts.Remove(assetPairId);
                }
                else
                {
                    _attempts[assetPairId] = new Tuple<int, int>(attempt.Item1, attempt.Item2 - 1);

                    _log.InfoWithDetails("Execution of hedge limit order is skipped", new
                    {
                        assetPairId,
                        volume,
                        positionType,
                        attempt = attempt.Item1,
                        iteration = attempt.Item2
                    });
                }
            }
            catch (Exception exception)
            {
                _attempts[assetPairId] =
                    new Tuple<int, int>(attempt.Item1 + 1, Math.Min(attempt.Item1 + 1, MaxIterations));

                _log.WarningWithDetails("Can not close positions.", exception,
                    new
                    {
                        assetPairId,
                        volume,
                        positionType,
                        attempt = attempt.Item1
                    });
            }

            var finishedAt = DateTime.UtcNow;

            _log.Info("HedgeService.ExecuteLimitOrderAsync() completed.", new
            {
                AssetPairId = assetPairId,
                StartedAt = startedAt,
                FinishedAt = finishedAt,
                Latency = (finishedAt - startedAt).TotalMilliseconds
            });

            return externalTrade;
        }

        private static int GetSign(PositionType positionType)
            => positionType == PositionType.Long ? 1 : -1;
    }
}
