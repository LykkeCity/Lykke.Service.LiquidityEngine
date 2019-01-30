using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Handlers;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPnLStopLossEngineService : IClosedPositionHandler
    {
        Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllAsync();

        Task AddAsync(PnLStopLossEngine pnLStopLossEngine);

        Task UpdateAsync(PnLStopLossEngine pnLStopLossEngine);

        Task DeleteAsync(string id);

        Task Initialize();

        Task ExecuteAsync();

        Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId);
    }
}
