using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ISettlementTradeRepository
    {
        Task<IReadOnlyCollection<SettlementTrade>> GetAllAsync();

        Task<SettlementTrade> GetByIdAsync(string settlementTradeId);

        Task InsertAsync(SettlementTrade settlementTrade);
        
        Task UpdateAsync(SettlementTrade settlementTrade);
    }
}
