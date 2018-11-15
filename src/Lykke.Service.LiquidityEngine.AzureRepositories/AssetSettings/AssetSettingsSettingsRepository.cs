using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.AssetSettings
{
    public class AssetSettingsSettingsRepository : IAssetSettingsRepository
    {
        private readonly INoSQLTableStorage<AssetSettingsEntity> _storage;

        public AssetSettingsSettingsRepository(INoSQLTableStorage<AssetSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<Domain.AssetSettings>> GetAllAsync()
        {
            IEnumerable<AssetSettingsEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<Domain.AssetSettings>>(entities);
        }

        public async Task InsertAsync(Domain.AssetSettings assetSettings)
        {
            var entity = new AssetSettingsEntity(GetPartitionKey(), GetRowKey(assetSettings.AssetId));

            Mapper.Map(assetSettings, entity);

            await _storage.InsertThrowConflictAsync(entity);
        }

        public async Task UpdateAsync(Domain.AssetSettings assetSettings)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(assetSettings.AssetId), entity =>
            {
                Mapper.Map(assetSettings, entity);
                return entity;
            });
        }

        public Task DeleteAsync(string assetId)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetId));
        }

        private static string GetPartitionKey()
            => "AssetSettings";

        private static string GetRowKey(string assetId)
            => assetId.ToUpper();
    }
}
