using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IRemainingVolumeRepository
    {
        Task<IReadOnlyCollection<RemainingVolume>> GetAllAsync();

        Task<RemainingVolume> GetByAssetPairAsync(string assetPairId);

        Task InsertAsync(RemainingVolume remainingVolume);

        Task UpdateAsync(RemainingVolume remainingVolume);

        Task DeleteAsync();

        Task DeleteAsync(string assetPairId);
    }
}
