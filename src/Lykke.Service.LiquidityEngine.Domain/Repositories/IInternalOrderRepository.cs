using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IInternalOrderRepository
    {
        Task<InternalOrder> GetByIdAsync(string internalOrderId);

        Task<IReadOnlyCollection<InternalOrder>> GetByPeriodAsync(DateTime startDate, DateTime endDate, int limit);

        Task<IReadOnlyCollection<InternalOrder>> GetByStatusAsync(InternalOrderStatus status);

        Task InsertAsync(InternalOrder internalOrder);

        Task UpdateAsync(InternalOrder internalOrder);
    }
}
