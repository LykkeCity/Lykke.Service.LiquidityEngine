using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IInternalTradeRepository
    {
        Task<IReadOnlyCollection<InternalTrade>>GetAsync(DateTime startDate, DateTime endDate, int limit);
        
        Task<InternalTrade> GetByIdAsync(string internalTradeId);

        Task InsertAsync(InternalTrade internalTrade);
    }
}
