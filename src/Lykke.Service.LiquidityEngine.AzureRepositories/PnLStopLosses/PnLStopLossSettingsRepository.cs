using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.AzureRepositories.Extensions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.PnLStopLosses
{
    public class PnLStopLossSettingsRepository : IPnLStopLossSettingsRepository
    {
        private readonly INoSQLTableStorage<PnLStopLossSettingsEntity> _storage;

        public PnLStopLossSettingsRepository(INoSQLTableStorage<PnLStopLossSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllAsync()
        {
            IEnumerable<PnLStopLossSettingsEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<PnLStopLossSettings>>(entities);
        }

        public async Task InsertAsync(PnLStopLossSettings pnLStopLossSettings)
        {
            var newEntity = new PnLStopLossSettingsEntity(GetPartitionKey(), GetRowKey(pnLStopLossSettings.Id));

            Mapper.Map(pnLStopLossSettings, newEntity);

            await _storage.InsertThrowConflictAsync(newEntity);
        }

        public Task DeleteAsync(string id)
        {
            return _storage.DeleteAsync(GetPartitionKey(), GetRowKey(id));
        }

        private static string GetPartitionKey()
            => "PnLStopLoss";

        private static string GetRowKey(string id)
            => id.ToUpper();
    }
}
