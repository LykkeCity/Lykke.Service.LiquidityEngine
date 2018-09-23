using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IBalanceOperationService
    {
        Task<IReadOnlyCollection<BalanceOperation>> GetAsync(DateTime startDate, DateTime endDate, int limit);

        Task AddAsync(BalanceOperation balanceOperation);
    }
}
