using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Positions
{
    public class RemainingVolumeRepository : IRemainingVolumeRepository
    {
        private readonly INoSQLTableStorage<RemainingVolumeEntity> _storage;

        public RemainingVolumeRepository(INoSQLTableStorage<RemainingVolumeEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<RemainingVolume>> GetAllAsync()
        {
            IList<RemainingVolumeEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<RemainingVolume[]>(entities);
        }

        public async Task<RemainingVolume> GetByAssetPairAsync(string assetPairId)
        {
            RemainingVolumeEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(assetPairId));

            return Mapper.Map<RemainingVolume>(entity);
        }

        public async Task InsertAsync(RemainingVolume remainingVolume)
        {
            var entity = new RemainingVolumeEntity(GetPartitionKey(), GetRowKey(remainingVolume.AssetPairId));

            Mapper.Map(remainingVolume, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task UpdateAsync(RemainingVolume remainingVolume)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(remainingVolume.AssetPairId), entity =>
            {
                Mapper.Map(remainingVolume, entity);
                return entity;
            });
        }

        public Task DeleteAsync()
        {
            return _storage.DeleteAsync();
        }

        public Task DeleteAsync(string assetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetPairId));
        }

        private static string GetPartitionKey()
            => "RemainingVolume";

        private static string GetRowKey(string assetPairId)
            => assetPairId.ToUpper();
    }
}
