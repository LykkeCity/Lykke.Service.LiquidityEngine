using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.CrossRateInstruments
{
    public class CrossRateInstrumentRepository : ICrossRateInstrumentRepository
    {
        private readonly INoSQLTableStorage<CrossRateInstrumentEntity> _storage;

        public CrossRateInstrumentRepository(INoSQLTableStorage<CrossRateInstrumentEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<CrossRateInstrument>> GetAllAsync()
        {
            IEnumerable<CrossRateInstrumentEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<CrossRateInstrument>>(entities);
        }

        public async Task InsertAsync(CrossRateInstrument instrument)
        {
            var entity = new CrossRateInstrumentEntity(GetPartitionKey(), GetRowKey(instrument.AssetPairId));

            Mapper.Map(instrument, entity);

            await _storage.InsertThrowConflictAsync(entity);
        }

        public async Task UpdateAsync(CrossRateInstrument instrument)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(instrument.AssetPairId), entity =>
            {
                Mapper.Map(instrument, entity);
                return entity;
            });
        }

        public Task DeleteAsync(string assetPairId)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(assetPairId));
        }

        private static string GetPartitionKey()
            => "CrossRateInstrument";

        private static string GetRowKey(string assetPairId)
            => assetPairId.ToUpper();
    }
}
