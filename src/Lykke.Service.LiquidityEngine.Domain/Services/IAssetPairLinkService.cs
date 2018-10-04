using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IAssetPairLinkService
    {
        Task<IReadOnlyCollection<AssetPairLink>> GetAllAsync();

        Task AddAsync(AssetPairLink assetPairLink);

        Task DeleteAsync(string assetPairId);
    }
}
