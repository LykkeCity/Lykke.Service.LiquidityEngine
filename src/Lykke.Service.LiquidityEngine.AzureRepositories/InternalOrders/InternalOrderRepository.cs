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

namespace Lykke.Service.LiquidityEngine.AzureRepositories.InternalOrders
{
    public class InternalOrderRepository : IInternalOrderRepository
    {
        private readonly INoSQLTableStorage<InternalOrderEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indicesStorage;

        public InternalOrderRepository(
            INoSQLTableStorage<InternalOrderEntity> storage,
            INoSQLTableStorage<AzureIndex> indicesStorage)
        {
            _storage = storage;
            _indicesStorage = indicesStorage;
        }

        public async Task<InternalOrder> GetByIdAsync(string internalOrderId)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(internalOrderId),
                GetIndexRowKey(internalOrderId));

            if (index == null)
                return null;

            InternalOrderEntity entity = await _storage.GetDataAsync(index);

            return Mapper.Map<InternalOrder>(entity);
        }

        public async Task<IReadOnlyCollection<InternalOrder>> GetByPeriodAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            var query = new TableQuery<InternalOrderEntity>().Where(filter).Take(limit);

            IEnumerable<InternalOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<InternalOrder>>(entities);
        }

        public async Task InsertAsync(InternalOrder internalOrder)
        {
            var entity =
                new InternalOrderEntity(GetPartitionKey(internalOrder.CreatedDate), GetRowKey(internalOrder.Id));

            Mapper.Map(internalOrder, entity);

            await _storage.InsertAsync(entity);

            AzureIndex index = new AzureIndex(GetIndexPartitionKey(internalOrder.Id), GetIndexRowKey(internalOrder.Id),
                entity);

            await _indicesStorage.InsertAsync(index);
        }

        public async Task<IReadOnlyCollection<InternalOrder>> GetByStatusAsync(InternalOrderStatus status)
        {
            string filter = TableQuery.GenerateFilterCondition(nameof(InternalOrderEntity.Status),
                QueryComparisons.Equal, status.ToString());

            var query = new TableQuery<InternalOrderEntity>().Where(filter);

            IEnumerable<InternalOrderEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<InternalOrder>>(entities);
        }

        public async Task UpdateAsync(InternalOrder internalOrder)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(internalOrder.Id),
                GetIndexRowKey(internalOrder.Id));

            if (index == null)
                throw new EntityNotFoundException();

            InternalOrderEntity entity = await _storage.GetDataAsync(index);
            
            Mapper.Map(internalOrder, entity);
            
            await _storage.InsertOrReplaceAsync(entity);
        }

        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(string internalOrderId)
            => internalOrderId;

        private static string GetIndexPartitionKey(string internalOrderId)
            => internalOrderId.CalculateHexHash32(4);

        private static string GetIndexRowKey(string internalOrderId)
            => internalOrderId;
    }
}
