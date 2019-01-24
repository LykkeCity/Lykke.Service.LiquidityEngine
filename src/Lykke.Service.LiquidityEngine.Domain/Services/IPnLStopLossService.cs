using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPnLStopLossService
    {
        Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllEnginesAsync();

        Task CreateAsync(PnLStopLossSettings pnLStopLossSettings);

        Task DeleteEngineAsync(string id);

        Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllGlobalSettingsAsync();

        Task ReapplyGlobalSettingsAsync(string id);

        Task DeleteGlobalSettingsAsync(string id);

        Task UpdateEngineModeAsync(string id, PnLStopLossEngineMode mode);

        Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId);
    }
}
