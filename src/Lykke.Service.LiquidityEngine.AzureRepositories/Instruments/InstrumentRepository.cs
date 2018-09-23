using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Instruments
{
    public class InstrumentRepository : IInstrumentRepository
    {
        private readonly INoSQLTableStorage<InstrumentEntity> _storage;

        public InstrumentRepository(INoSQLTableStorage<InstrumentEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<Instrument>> GetAllAsync()
        {
            IEnumerable<InstrumentEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<Instrument>>(entities);
        }

        public async Task InsertAsync(Instrument instrument)
        {
            var entity = new InstrumentEntity(GetPartitionKey(), GetRowKey(instrument.AssetPairId));

            Mapper.Map(instrument, entity);

            await _storage.InsertThrowConflictAsync(entity);
        }

        public async Task UpdateAsync(Instrument instrument)
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
            => "Instrument";

        private static string GetRowKey(string assetPairId)
            => assetPairId.ToUpper();
    }
}
