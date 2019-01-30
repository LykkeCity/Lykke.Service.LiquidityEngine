using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPnLStopLossSettingsService
    {
        Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllAsync();

        Task AddAsync(PnLStopLossSettings pnLStopLossSettings);

        Task RefreshAsync(string id);

        Task DeleteAsync(string id);
    }
}
