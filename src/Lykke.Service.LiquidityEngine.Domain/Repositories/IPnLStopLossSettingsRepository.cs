using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IPnLStopLossSettingsRepository
    {
        Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllAsync();
        
        Task InsertAsync(PnLStopLossSettings pnLStopLossSettings);
        
        Task DeleteAsync(string id);
    }
}
