using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.PnLStopLosses
{
    public class PnLStopLossEngineRepository : IPnLStopLossEngineRepository
    {
        private readonly INoSQLTableStorage<PnLStopLossEngineEntity> _storage;

        public PnLStopLossEngineRepository(INoSQLTableStorage<PnLStopLossEngineEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllAsync()
        {
            IEnumerable<PnLStopLossEngineEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<PnLStopLossEngine>>(entities);
        }

        public async Task UpdateAsync(PnLStopLossEngine pnLStopLossEngine)
        {
            await _storage.MergeAsync(GetPartitionKey(), GetRowKey(pnLStopLossEngine.Id), entity =>
            {
                Mapper.Map(pnLStopLossEngine, entity);
                return entity;
            });
        }

        public async Task InsertAsync(PnLStopLossEngine pnLStopLossEngine)
        {
            var newEntity = new PnLStopLossEngineEntity(GetPartitionKey(), GetRowKey(pnLStopLossEngine.Id));

            Mapper.Map(pnLStopLossEngine, newEntity);
            
            await _storage.InsertThrowConflictAsync(newEntity);
        }

        public Task DeleteAsync(string id)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(id));
        }

        private static string GetPartitionKey()
            => "PnLStopLossEngine";

        private static string GetRowKey(string id)
            => id.ToUpper();
    }
}
