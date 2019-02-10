using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IAssetSettingsService
    {
        Task<IReadOnlyCollection<AssetSettings>> GetAllAsync();
        
        Task<AssetSettings> GetByIdAsync(string assetId);

        Task<Domain.AssetSettings> GetByLykkeIdAsync(string lykkeAssetId);

        Task AddAsync(AssetSettings assetSettings);
        
        Task UpdateAsync(AssetSettings assetSettings);
        
        Task DeleteAsync(string assetId);
    }
}
