using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IInternalOrderService
    {
        Task<InternalOrder> GetByIdAsync(string internalOrderId);

        Task<IReadOnlyCollection<InternalOrder>> GetByPeriodAsync(DateTime startDate, DateTime endDate, int limit);

        Task<string> CreateOrderAsync(string walletId, string assetPairId, LimitOrderType type, decimal price,
            decimal volume, bool fullExecution);
    }
}
