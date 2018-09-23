using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IInstrumentService
    {
        Task<IReadOnlyCollection<Instrument>> GetAllAsync();
        
        Task<Instrument> GetByAssetPairIdAsync(string assetPairId);
        
        Task AddAsync(Instrument instrument);
        
        Task UpdateAsync(Instrument instrument);
        
        Task DeleteAsync(string assetPairId);
        
        Task AddLevelAsync(string assetPairId, LevelVolume levelVolume);
        
        Task UpdateLevelAsync(string assetPairId, LevelVolume levelVolume);
        
        Task RemoveLevelAsync(string assetPairId, int levelNumber);
    }
}
