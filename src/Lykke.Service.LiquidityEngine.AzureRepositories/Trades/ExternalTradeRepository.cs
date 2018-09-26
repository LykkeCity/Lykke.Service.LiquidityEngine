using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using AzureStorage.Tables.Templates.Index;
using Common;
using Lykke.AzureStorage.Tables;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Trades
{
    public class ExternalTradeRepository : IExternalTradeRepository
    {
        private readonly INoSQLTableStorage<ExternalTradeEntity> _storage;
        private readonly INoSQLTableStorage<AzureIndex> _indicesStorage;

        public ExternalTradeRepository(
            INoSQLTableStorage<ExternalTradeEntity> storage,
            INoSQLTableStorage<AzureIndex> indicesStorage)
        {
            _storage = storage;
            _indicesStorage = indicesStorage;
        }

        public async Task<IReadOnlyCollection<ExternalTrade>> GetAsync(DateTime startDate, DateTime endDate, int limit)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(AzureTableEntity.PartitionKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            var query = new TableQuery<ExternalTradeEntity>().Where(filter).Take(limit);

            IEnumerable<ExternalTradeEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<ExternalTrade>>(entities);
        }

        public async Task<ExternalTrade> GetByIdAsync(string externalTradeId)
        {
            AzureIndex index = await _indicesStorage.GetDataAsync(GetIndexPartitionKey(externalTradeId),
                GetIndexRowKey(externalTradeId));

            if (index == null)
                return null;

            ExternalTradeEntity entity = await _storage.GetDataAsync(index);

            return Mapper.Map<ExternalTrade>(entity);
        }

        public async Task InsertAsync(ExternalTrade externalTrade)
        {
            var entity = new ExternalTradeEntity(GetPartitionKey(externalTrade.Time), GetRowKey(externalTrade.Id));

            Mapper.Map(externalTrade, entity);

            await _storage.InsertAsync(entity);

            AzureIndex index = new AzureIndex(GetIndexPartitionKey(externalTrade.Id), GetRowKey(externalTrade.Id),
                entity);

            await _indicesStorage.InsertAsync(index);
        }
        
        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(string externalTradeId)
            => externalTradeId;

        private static string GetIndexPartitionKey(string externalTradeId)
            => externalTradeId.CalculateHexHash32(4);

        private static string GetIndexRowKey(string externalTradeId)
            => externalTradeId;
    }
}
