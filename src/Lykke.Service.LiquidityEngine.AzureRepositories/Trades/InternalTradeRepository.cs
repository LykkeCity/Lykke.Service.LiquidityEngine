using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Trades
{
    public class InternalTradeRepository : IInternalTradeRepository
    {
        private readonly INoSQLTableStorage<InternalTradeEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indicesStorage;

        public InternalTradeRepository(
            INoSQLTableStorage<InternalTradeEntity> storage,
            INoSQLTableStorage<AzureIndex> indicesStorage)
        {
            _storage = storage;
            _indicesStorage = indicesStorage;
        }

        public async Task<IReadOnlyCollection<InternalTrade>> GetAsync(DateTime startDate, DateTime endDate, int limit)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(InternalTradeEntity.RowKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(InternalTradeEntity.RowKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            var query = new TableQuery<InternalTradeEntity>().Where(filter).Take(limit);

            IEnumerable<InternalTradeEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<InternalTrade>>(entities);
        }

        public async Task<InternalTrade> GetByIdAsync(string internalTradeId)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(internalTradeId),
                GetIndexRowKey(internalTradeId));

            if (index == null)
                return null;

            InternalTradeEntity entity = await _storage.GetDataAsync(index);

            return Mapper.Map<InternalTrade>(entity);
        }

        public async Task InsertAsync(InternalTrade internalTrade)
        {
            var entity = new InternalTradeEntity(GetPartitionKey(internalTrade.Time), GetRowKey(internalTrade.Id));

            Mapper.Map(internalTrade, entity);

            await _storage.InsertAsync(entity);

            AzureIndex index = new AzureIndex(GetIndexPartitionKey(internalTrade.Id), GetRowKey(internalTrade.Id),
                entity);

            await _indicesStorage.InsertAsync(index);
        }
        
        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(string internalTradeId)
            => internalTradeId;

        private static string GetIndexPartitionKey(string internalTradeId)
            => internalTradeId.CalculateHexHash32(4);

        private static string GetIndexRowKey(string internalTradeId)
            => internalTradeId;
    }
}
