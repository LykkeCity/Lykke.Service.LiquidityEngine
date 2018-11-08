using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Positions
{
    public class PositionRepository : IPositionRepository
    {
        private readonly INoSQLTableStorage<PositionEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indicesStorage;

        public PositionRepository(
            INoSQLTableStorage<PositionEntity> storage,
            INoSQLTableStorage<AzureIndex> indicesStorage)
        {
            _storage = storage;
            _indicesStorage = indicesStorage;
        }

        public async Task<IReadOnlyCollection<Position>> GetAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            if (!string.IsNullOrEmpty(assetPairId))
            {
                filter = TableQuery.CombineFilters(filter, TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(PositionEntity.AssetPairId), QueryComparisons.Equal,
                        assetPairId));
            }

            if (!string.IsNullOrEmpty(tradeAssetPairId))
            {
                filter = TableQuery.CombineFilters(filter, TableOperators.And,
                    TableQuery.GenerateFilterCondition(nameof(PositionEntity.TradeAssetPairId), QueryComparisons.Equal,
                        tradeAssetPairId));
            }

            var query = new TableQuery<PositionEntity>().Where(filter).Take(limit);

            IEnumerable<PositionEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<Position>>(entities);
        }

        public async Task<Position> GetByIdAsync(string positionId)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(positionId),
                GetIndexRowKey(positionId));

            if (index == null)
                return null;

            PositionEntity entity = await _storage.GetDataAsync(index);

            return Mapper.Map<Position>(entity);
        }

        public async Task InsertAsync(Position position)
        {
            var entity = new PositionEntity(GetPartitionKey(position.Date), GetRowKey(position.Id));

            Mapper.Map(position, entity);

            await _storage.InsertAsync(entity);

            AzureIndex index = new AzureIndex(GetIndexPartitionKey(position.Id), GetIndexRowKey(position.Id), entity);

            await _indicesStorage.InsertAsync(index);
        }

        public async Task UpdateAsync(Position position)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(position.Id),
                GetIndexRowKey(position.Id));

            if (index == null)
                throw new EntityNotFoundException();

            await _storage.MergeAsync(index.PrimaryPartitionKey, index.PrimaryRowKey, entity =>
            {
                Mapper.Map(position, entity);
                return entity;
            });
        }

        public async Task DeleteAsync()
        {
            await _storage.DeleteAsync();
            await _indicesStorage.DeleteAsync();
        }

        public async Task DeleteAsync(string positionId)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(positionId),
                GetIndexRowKey(positionId));

            if (index == null)
                throw new EntityNotFoundException();
            
            await _storage.DeleteAsync(index.PrimaryPartitionKey, index.PrimaryRowKey);
            await _indicesStorage.DeleteAsync(index);
        }

        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(string positionId)
            => positionId;

        private static string GetIndexPartitionKey(string positionId)
            => positionId.CalculateHexHash32(4);

        private static string GetIndexRowKey(string positionId)
            => positionId;
    }
}
