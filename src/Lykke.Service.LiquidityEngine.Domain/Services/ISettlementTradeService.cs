using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ISettlementTradeService
    {
        Task<IReadOnlyCollection<SettlementTrade>> GetAllAsync();

        Task<SettlementTrade> GetByIdAsync(string settlementTradeId);

        Task UpdateAsync(SettlementTrade settlementTrade);
        
        Task FindTradesAsync();
    }
}
