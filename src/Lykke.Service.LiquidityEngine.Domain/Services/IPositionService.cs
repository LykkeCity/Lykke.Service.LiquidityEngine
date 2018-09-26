using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPositionService
    {
        Task<IReadOnlyCollection<Position>> GetAllAsync(DateTime startDate, DateTime endDate, int limit);

        Task<IReadOnlyCollection<Position>> GetOpenedAsync(string assetPairId);

        Task OpenPositionAsync(IReadOnlyCollection<InternalTrade> internalTrades);

        Task ClosePositionAsync(ExternalTrade externalTrade);
    }
}
