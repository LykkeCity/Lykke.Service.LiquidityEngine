using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IExternalTradeRepository
    {
        Task<IReadOnlyCollection<ExternalTrade>>GetAsync(DateTime startDate, DateTime endDate, int limit);
        
        Task<ExternalTrade> GetByIdAsync(string externalTradeId);

        Task InsertAsync(ExternalTrade externalTrade);
    }
}
