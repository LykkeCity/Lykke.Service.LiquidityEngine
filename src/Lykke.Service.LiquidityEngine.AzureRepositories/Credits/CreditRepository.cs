using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Credits
{
    public class CreditRepository : ICreditRepository
    {
        private readonly INoSQLTableStorage<CreditEntity> _storage;

        public CreditRepository(INoSQLTableStorage<CreditEntity> storage)
        {
            _storage = storage;
        }
        
        public async Task<IReadOnlyCollection<Credit>> GetAllAsync()
        {
            IEnumerable<CreditEntity> entities = await _storage.GetDataAsync(GetPartitionKey());

            return Mapper.Map<List<Credit>>(entities);
        }

        public async Task InsertOrReplaceAsync(Credit credit)
        {
            var entity = new CreditEntity(GetPartitionKey(), GetRowKey(credit.AssetId));

            Mapper.Map(credit, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }
        
        private static string GetPartitionKey()
            => "Credit";

        private static string GetRowKey(string assetId)
            => assetId.ToUpper();
    }
}
