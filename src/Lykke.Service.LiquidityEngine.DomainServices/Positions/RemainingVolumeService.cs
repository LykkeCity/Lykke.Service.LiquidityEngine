using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Positions
{
    [UsedImplicitly]
    public class RemainingVolumeService : IRemainingVolumeService
    {
        private readonly IRemainingVolumeRepository _remainingVolumeRepository;
        private readonly InMemoryCache<RemainingVolume> _cache;
        private readonly ILog _log;

        public RemainingVolumeService(IRemainingVolumeRepository remainingVolumeRepository, ILogFactory logFactory)
        {
            _remainingVolumeRepository = remainingVolumeRepository;
            _cache = new InMemoryCache<RemainingVolume>(remainingVolume => remainingVolume.AssetPairId, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<RemainingVolume>> GetAllAsync()
        {
            IReadOnlyCollection<RemainingVolume> remainingVolumes = _cache.GetAll();

            if (remainingVolumes == null)
            {
                remainingVolumes = await _remainingVolumeRepository.GetAllAsync();

                _cache.Initialize(remainingVolumes);
            }

            return remainingVolumes;
        }

        public async Task RegisterVolumeAsync(string assetPairId, decimal volume)
        {
            IReadOnlyCollection<RemainingVolume> remainingVolumes = await GetAllAsync();

            RemainingVolume remainingVolume = remainingVolumes.SingleOrDefault(o => o.AssetPairId == assetPairId);

            bool isNew = false;

            if (remainingVolume == null)
            {
                remainingVolume = new RemainingVolume {AssetPairId = assetPairId};
                isNew = true;
            }

            remainingVolume.Add(volume);

            if (isNew)
                await _remainingVolumeRepository.InsertAsync(remainingVolume);
            else
                await _remainingVolumeRepository.UpdateAsync(remainingVolume);

            _cache.Set(remainingVolume);
        }

        public async Task CloseVolumeAsync(string assetPairId, decimal volume)
        {
            IReadOnlyCollection<RemainingVolume> remainingVolumes = await GetAllAsync();

            RemainingVolume remainingVolume = remainingVolumes.SingleOrDefault(o => o.AssetPairId == assetPairId);

            if (remainingVolume == null)
            {
                _log.WarningWithDetails("Remaining volume not exist", assetPairId);
                return;
            }

            remainingVolume.Subtract(volume);

            await _remainingVolumeRepository.UpdateAsync(remainingVolume);

            _cache.Set(remainingVolume);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            await _remainingVolumeRepository.DeleteAsync(assetPairId);

            _cache.Remove(assetPairId);
        }
    }
}
