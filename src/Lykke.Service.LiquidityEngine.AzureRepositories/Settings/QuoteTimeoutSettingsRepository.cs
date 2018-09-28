using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Settings
{
    public class QuoteTimeoutSettingsRepository : IQuoteTimeoutSettingsRepository
    {
        private readonly INoSQLTableStorage<QuoteTimeoutSettingsEntity> _storage;

        public QuoteTimeoutSettingsRepository(INoSQLTableStorage<QuoteTimeoutSettingsEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task<QuoteTimeoutSettings> GetAsync()
        {
            QuoteTimeoutSettingsEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey());

            return Mapper.Map<QuoteTimeoutSettings>(entity);
        }

        public async Task InsertOrReplaceAsync(QuoteTimeoutSettings quoteTimeoutSettings)
        {
            var entity = new QuoteTimeoutSettingsEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(quoteTimeoutSettings, entity);
            
            await _storage.InsertOrReplaceAsync(entity);
        }
        
        private static string GetPartitionKey()
            => "QuoteTimeout";

        private static string GetRowKey()
            => "QuoteTimeout";
    }
}
