using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IAssetSettingsRepository
    {
        Task<IReadOnlyCollection<AssetSettings>> GetAllAsync();
        
        Task InsertAsync(AssetSettings assetSettings);
        
        Task UpdateAsync(AssetSettings assetSettings);
        
        Task DeleteAsync(string assetId);
    }
}
