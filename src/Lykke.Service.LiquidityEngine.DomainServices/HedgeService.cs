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
        private readonly ILog _log;

        public HedgeService(
            IPositionService positionService,
            IInstrumentService instrumentService,
            IExternalExchangeService externalExchangeService,
            IMarketMakerStateService marketMakerStateService,
            ILogFactory logFactory)
        {
            _positionService = positionService;
            _instrumentService = instrumentService;
            _externalExchangeService = externalExchangeService;
            _marketMakerStateService = marketMakerStateService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync()
        {
            MarketMakerState state = await _marketMakerStateService.GetStateAsync();

            if (state.Status != MarketMakerStatus.Active)
            {
                return;
            }

            IReadOnlyCollection<Position> positions = await _positionService.GetOpenedAsync();

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Long), PositionType.Long);

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Short), PositionType.Short);
        }

        private async Task ClosePositionsAsync(IEnumerable<Position> positions, PositionType positionType)
        {
            foreach (IGrouping<string, Position> group in positions.GroupBy(o => o.AssetPairId))
            {
                Instrument instrument = await _instrumentService.GetByAssetPairIdAsync(group.Key);
                
                decimal volume = Math.Round(group.Sum(o => o.Volume), instrument.VolumeAccuracy);

                ExternalTrade externalTrade = null;

                try
                {
                    if (positionType == PositionType.Long)
                        externalTrade = await _externalExchangeService.ExecuteSellLimitOrderAsync(group.Key, volume);
                    else
                        externalTrade = await _externalExchangeService.ExecuteBuyLimitOrderAsync(group.Key, volume);
                }
                catch (ExternalExchangeThrottlingException exception)
                {
                    _log.WarningWithDetails("Can not close positions because of throttling", exception,
                        new { assetPairId = group.Key, volume });
                }
                catch (ExternalExchangeException exception)
                {
                    _log.WarningWithDetails(
                        "Can not close positions because of integration error. Setting market maker state to error",
                        exception, new { assetPairId = group.Key, volume });

                    await SetError(MarketMakerError.IntegrationError,
                        $"An error occurred during position closing ({group.Key}): {exception.Message}");
                    break;
                }
                catch (Exception exception)
                {
                    _log.WarningWithDetails("Can not close positions", exception,
                        new { assetPairId = group.Key, volume });
                }

                if (externalTrade != null)
                {
                    foreach (Position position in group)
                        await _positionService.ClosePositionAsync(position, externalTrade);
                }
            }
        }

        private Task SetError(MarketMakerError error, string message)
        {
            return _marketMakerStateService.SetStateAsync(
                new MarketMakerState
                {
                    Status = MarketMakerStatus.Error,
                    Error = error,
                    ErrorMessage = message,
                    Time = DateTime.UtcNow
                },
                $"Reason: exception on executing limit order {message}");
        }
    }
}
