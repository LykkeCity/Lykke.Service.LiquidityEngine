using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.AssetPairLinks
{
    public class AssetPairLinkRepository : IAssetPairLinkRepository
    {
        private readonly INoSQLTableStorage<AssetPairLinkEntity> _storage;

        public AssetPairLinkRepository(INoSQLTableStorage<AssetPairLinkEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<AssetPairLink>> GetAllAsync()
        {
            IEnumerable<AssetPairLinkEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<AssetPairLink>>(entities);
        }

        public async Task AddAsync(AssetPairLink assetPairLink)
        {
            var entity = new AssetPairLinkEntity(GetPartitionKey(), GetRowKey(assetPairLink.AssetPairId));

            Mapper.Map(assetPairLink, entity);

            await _storage.InsertThrowConflictAsync(entity);
        }

        public Task DeleteAsync(string assetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetPairId));
        }

        private static string GetPartitionKey()
            => "AssetPairLink";

        private static string GetRowKey(string assetPairId)
            => assetPairId.ToUpper();
    }
}
