using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public class HedgeService : IHedgeService
    {
        private readonly IPositionService _positionService;
        private readonly IInstrumentService _instrumentService;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly IMarketMakerStateService _marketMakerStateService;
        private readonly IRemainingVolumeService _remainingVolumeService;
        private readonly ILog _log;

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

        public async Task ExecuteAsync()
        {
            MarketMakerState state = await _marketMakerStateService.GetStateAsync();

            if (state.Status != MarketMakerStatus.Active)
                return;

            IReadOnlyCollection<Position> positions = await _positionService.GetOpenAllAsync();

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Long), PositionType.Long);

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Short), PositionType.Short);

            await CloseRemainingVolumeAsync();
        }

        private async Task ClosePositionsAsync(IEnumerable<Position> positions, PositionType positionType)
        {
            foreach (IGrouping<string, Position> group in positions.GroupBy(o => o.AssetPairId))
            {
                MarketMakerState marketMakerState = await _marketMakerStateService.GetStateAsync();

                if (marketMakerState.Status != MarketMakerStatus.Active)
                    continue;

                Instrument instrument = await _instrumentService.GetByAssetPairIdAsync(group.Key);

                decimal originalVolume = group.Sum(o => o.Volume);

                decimal volume = Math.Round(originalVolume, instrument.VolumeAccuracy);

                ExternalTrade externalTrade = await ExecuteLimitOrderAsync(group.Key, volume, positionType);

                if (externalTrade != null)
                {
                    foreach (Position position in group)
                        await _positionService.CloseAsync(position, externalTrade);

                    await _remainingVolumeService.RegisterVolumeAsync(group.Key, originalVolume - volume);
                }
            }
        }

        private async Task CloseRemainingVolumeAsync()
        {
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

                await _positionService.CloseRemainingVolumeAsync(instrument.AssetPairId, externalTrade);

                if (externalTrade != null)
                {
                    await _remainingVolumeService.RegisterVolumeAsync(instrument.AssetPairId,
                        remainingVolume.Volume - volume);
                }
            }
        }

        private async Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume,
            PositionType positionType)
        {
            ExternalTrade externalTrade = null;

            try
            {
                if (positionType == PositionType.Long)
                    externalTrade = await _externalExchangeService.ExecuteSellLimitOrderAsync(assetPairId, volume);
                else
                    externalTrade = await _externalExchangeService.ExecuteBuyLimitOrderAsync(assetPairId, volume);
            }
            catch (ExternalExchangeThrottlingException exception)
            {
                _log.WarningWithDetails("Can not close positions because of throttling", exception,
                    new {assetPairId, volume});
            }
            catch (ExternalExchangeException exception)
            {
                _log.WarningWithDetails(
                    "Can not close positions because of integration error. Setting market maker state to error",
                    exception, new {assetPairId, volume});

                await _marketMakerStateService.SetStateAsync(MarketMakerError.IntegrationError,
                    $"An error occurred while position closing ({assetPairId}): {exception.Message}",
                    "An error occurred while executing limit order");
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("Can not close positions", exception, new {assetPairId, volume});
            }

            return externalTrade;
        }
    }
}
