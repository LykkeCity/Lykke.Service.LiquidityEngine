using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IBalanceOperationRepository
    {
        Task<IReadOnlyCollection<BalanceOperation>> GetAsync(DateTime startDate, DateTime endDate, int limit);

        Task InsertAsync(BalanceOperation balanceOperation);
    }
}
