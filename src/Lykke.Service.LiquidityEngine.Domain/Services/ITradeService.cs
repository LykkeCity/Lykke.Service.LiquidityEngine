using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ITradeService
    {
        Task<IReadOnlyCollection<ExternalTrade>>GetExternalTradesAsync(DateTime startDate, DateTime endDate, int limit);

        Task<ExternalTrade> GetExternalTradeByIdAsync(string tradeId);

        Task<IReadOnlyCollection<InternalTrade>>GetInternalTradesAsync(DateTime startDate, DateTime endDate, int limit);

        Task<InternalTrade> GetInternalTradeByIdAsync(string tradeId);

        Task RegisterAsync(IReadOnlyCollection<InternalTrade> internalTrades);

        Task RegisterAsync(ExternalTrade externalTrade);
    }
}
