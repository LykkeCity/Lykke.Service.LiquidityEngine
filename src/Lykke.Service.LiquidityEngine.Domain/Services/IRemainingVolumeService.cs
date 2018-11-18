using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IRemainingVolumeService
    {
        Task<IReadOnlyCollection<RemainingVolume>> GetAllAsync();

        Task RegisterVolumeAsync(string assetPairId, decimal volume);

        Task CloseVolumeAsync(string assetPairId, decimal volume);

        Task DeleteAsync(string assetPairId);
    }
}
