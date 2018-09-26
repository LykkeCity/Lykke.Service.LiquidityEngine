using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Positions
{
    public class PositionService : IPositionService
    {
        public Task<IReadOnlyCollection<Position>> GetAllAsync(DateTime startDate, DateTime endDate, int limit)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyCollection<Position>> GetOpenedAsync(string assetPairId)
        {
            throw new NotImplementedException();
        }

        public Task OpenPositionAsync(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            throw new NotImplementedException();
        }

        public Task ClosePositionAsync(ExternalTrade externalTrade)
        {
            throw new NotImplementedException();
        }
    }
}
