using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.VersionControl
{
    public class VersionControlRepository : IVersionControlRepository
    {
        private readonly INoSQLTableStorage<SystemVersionEntity> _storage;

        public VersionControlRepository(INoSQLTableStorage<SystemVersionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<SystemVersion> GetAsync()
        {
            SystemVersionEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey());

            return entity != null
                ? Mapper.Map<SystemVersion>(entity)
                : new SystemVersion { VersionNumber = 0 };
        }

        public async Task UpdateAsync(SystemVersion systemVersion)
        {
            var entity = new SystemVersionEntity(GetPartitionKey(), GetRowKey());

            Mapper.Map(systemVersion, entity);

            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey()
            => "Version";

        private static string GetRowKey()
            => "Version";
    }
}
