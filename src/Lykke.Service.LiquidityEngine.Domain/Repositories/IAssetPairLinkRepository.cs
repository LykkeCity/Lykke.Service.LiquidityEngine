using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IAssetPairLinkRepository
    {
        Task<IReadOnlyCollection<AssetPairLink>> GetAllAsync();

        Task InsertAsync(AssetPairLink assetPairLink);
        
        Task UpdateAsync(AssetPairLink assetPairLink);

        Task DeleteAsync(string assetPairId);
    }
}
