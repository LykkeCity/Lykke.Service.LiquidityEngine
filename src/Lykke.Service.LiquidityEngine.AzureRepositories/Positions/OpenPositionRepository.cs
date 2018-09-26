using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Common;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Positions
{
    public class OpenPositionRepository : IOpenPositionRepository
    {
        private readonly INoSQLTableStorage<PositionEntity> _storage;

        public OpenPositionRepository(INoSQLTableStorage<PositionEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<Position>> GetAllAsync()
        {
            IList<PositionEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<Position[]>(entities);
        }

        public async Task<IReadOnlyCollection<Position>> GetByAssetPairIdAsync(string assetPairId)
        {
            IEnumerable<PositionEntity> entities = await _storage.GetDataAsync(GetPartitionKey(assetPairId));

            return Mapper.Map<Position[]>(entities);
        }

        public async Task InsertAsync(Position position)
        {
            var entity = new PositionEntity(GetPartitionKey(position.AssetPairId), GetRowKey(position.Id));

            Mapper.Map(position, entity);

            await _storage.InsertAsync(entity);
        }

        public Task DeleteAsync(string assetPairId, string positionId)
        {
            return _storage.DeleteAsync(GetPartitionKey(assetPairId), GetRowKey(positionId));
        }

        private static string GetPartitionKey(string assetPairId)
            => assetPairId;

        private static string GetRowKey(string positionId)
            => positionId;
    }
}
