using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Handlers;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPnLStopLossService : IClosedPositionHandler
    {
        Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllSettingsAsync();

        Task AddSettingsAsync(PnLStopLossSettings pnLStopLossSettings);

        Task RefreshSettingsAsync(string id);

        Task DeleteSettingsAsync(string id);


        Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllEnginesAsync();

        Task UpdateEngineAsync(PnLStopLossEngine pnLStopLossEngine);

        Task DeleteEngineAsync(string id);


        Task Initialize();

        Task ExecuteAsync();

        Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId);
    }
}
