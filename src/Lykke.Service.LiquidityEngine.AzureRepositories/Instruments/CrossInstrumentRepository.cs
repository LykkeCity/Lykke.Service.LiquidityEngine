using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Instruments
{
    public class CrossInstrumentRepository : ICrossInstrumentRepository
    {
        private readonly INoSQLTableStorage<CrossInstrumentEntity> _storage;

        public CrossInstrumentRepository(INoSQLTableStorage<CrossInstrumentEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<CrossInstrument>> GetAsync(string assetPairId)
        {
            IEnumerable<CrossInstrumentEntity> entities = await _storage.GetDataAsync(GetPartitionKey(assetPairId));

            return Mapper.Map<CrossInstrument[]>(entities);
        }

        public async Task AddAsync(string assetPairId, CrossInstrument crossInstrument)
        {
            var entity = new CrossInstrumentEntity(GetPartitionKey(assetPairId), GetRowKey(crossInstrument.AssetPairId));

            Mapper.Map(crossInstrument, entity);

            await _storage.InsertAsync(entity);
        }

        public async Task UpdateAsync(string assetPairId, CrossInstrument crossInstrument)
        {
            await _storage.MergeAsync(GetPartitionKey(assetPairId), GetRowKey(crossInstrument.AssetPairId),
                entity =>
                {
                    Mapper.Map(crossInstrument, entity);
                    return entity;
                });
        }

        public Task DeleteAsync(string assetPairId, string crossAssetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(assetPairId), GetRowKey(crossAssetPairId));
        }

        private static string GetPartitionKey(string assetPairId)
            => assetPairId.ToUpper();

        private static string GetRowKey(string crossAssetPairId)
            => crossAssetPairId.ToUpper();
    }
}
