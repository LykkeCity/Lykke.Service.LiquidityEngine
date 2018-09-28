using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices
{
    public class HedgeService : IHedgeService
    {
        private readonly IPositionService _positionService;
        private readonly IExternalExchangeService _externalExchangeService;
        private readonly ILog _log;

        public HedgeService(
            IPositionService positionService,
            IExternalExchangeService externalExchangeService,
            ILogFactory logFactory)
        {
            _positionService = positionService;
            _externalExchangeService = externalExchangeService;
            _log = logFactory.CreateLog(this);
        }

        public async Task ExecuteAsync()
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetOpenedAsync();

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Long), PositionType.Long);

            await ClosePositionsAsync(positions.Where(o => o.Type == PositionType.Short), PositionType.Short);
        }

        private async Task ClosePositionsAsync(IEnumerable<Position> positions, PositionType positionType)
        {
            foreach (IGrouping<string, Position> group in positions.GroupBy(o => o.AssetPairId))
            {
                decimal volume = group.Sum(o => o.Volume);

                ExternalTrade externalTrade;

                try
                {
                    if (positionType == PositionType.Long)
                        externalTrade = await _externalExchangeService.ExecuteSellLimitOrderAsync(group.Key, volume);
                    else
                        externalTrade = await _externalExchangeService.ExecuteBuyLimitOrderAsync(group.Key, volume);
                }
                catch (Exception exception)
                {
                    _log.WarningWithDetails("Can not close positions", exception,
                        new {assetPairId = group.Key, volume});

                    externalTrade = null;
                }

                if (externalTrade != null)
                {
                    foreach (Position position in group)
                        await _positionService.ClosePositionAsync(position, externalTrade);
                }
            }
        }
    }
}
