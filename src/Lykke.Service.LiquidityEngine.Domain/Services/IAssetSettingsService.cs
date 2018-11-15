using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IAssetSettingsService
    {
        Task<IReadOnlyCollection<AssetSettings>> GetAllAsync();
        
        Task<AssetSettings> GetByIdAsync(string assetId);
        
        Task AddAsync(AssetSettings assetSettings);
        
        Task UpdateAsync(AssetSettings assetSettings);
        
        Task DeleteAsync(string assetId);

        Task<(decimal?, decimal?)> ConvertAmountAsync(string assetId, decimal amount);
    }
}
