using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ITradeService
    {
        DateTime GetLastInternalTradeTime(string assetPairId);
        
        Task<IReadOnlyCollection<ExternalTrade>>GetExternalTradesAsync(DateTime startDate, DateTime endDate, int limit);

        Task<ExternalTrade> GetExternalTradeByIdAsync(string externalTradeId);

        Task<IReadOnlyCollection<InternalTrade>>GetInternalTradesAsync(DateTime startDate, DateTime endDate, int limit);

        Task<InternalTrade> GetInternalTradeByIdAsync(string internalTradeId);

        Task RegisterAsync(IReadOnlyCollection<InternalTrade> internalTrades);

        Task RegisterAsync(ExternalTrade externalTrade);
    }
}
