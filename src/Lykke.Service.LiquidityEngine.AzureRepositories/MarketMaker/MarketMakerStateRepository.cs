using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.MarketMaker
{
    public class MarketMakerStateRepository : IMarketMakerStateRepository
    {
        private readonly INoSQLTableStorage<MarketMakerStateEntity> _storage;

        public MarketMakerStateRepository(INoSQLTableStorage<MarketMakerStateEntity> storage)
        {
            _storage = storage;
        }

        public async Task<MarketMakerState> GetAsync()
        {
            MarketMakerStateEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey());

            return Mapper.Map<MarketMakerState>(entity);
        }

        public async Task InsertOrReplaceAsync(MarketMakerState state)
        {
            var entity = new MarketMakerStateEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(state, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey()
            => "MarketMakerState";

        private static string GetRowKey()
            => "MarketMakerState";
    }
}
