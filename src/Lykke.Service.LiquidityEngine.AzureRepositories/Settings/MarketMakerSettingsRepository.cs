using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    public class MarketMakerSettingsRepository : IMarketMakerSettingsRepository
    {
        private readonly INoSQLTableStorage<MarketMakerSettingsEntity> _storage;

        public MarketMakerSettingsRepository(INoSQLTableStorage<MarketMakerSettingsEntity> storage)
        {
            _storage = storage;
        }

        public async Task<MarketMakerSettings> GetAsync()
        {
            MarketMakerSettingsEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey());

            return Mapper.Map<MarketMakerSettings>(entity);
        }

        public async Task InsertOrReplaceAsync(MarketMakerSettings marketMakerSettings)
        {
            var entity = new MarketMakerSettingsEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(marketMakerSettings, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey()
            => "MarketMaker";

        private static string GetRowKey()
            => "MarketMaker";
    }
}
