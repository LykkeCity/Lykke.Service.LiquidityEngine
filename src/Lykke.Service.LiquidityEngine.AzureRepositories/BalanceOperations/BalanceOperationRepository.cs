using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.BalanceOperations
{
    public class BalanceOperationRepository : IBalanceOperationRepository
    {
        private readonly INoSQLTableStorage<BalanceOperationEntity> _storage;

        public BalanceOperationRepository(INoSQLTableStorage<BalanceOperationEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<BalanceOperation>> GetAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            string filter = TableQuery.CombineFilters(
                TableQuery.GenerateFilterCondition(nameof(BalanceOperationEntity.RowKey), QueryComparisons.GreaterThan,
                    GetPartitionKey(endDate.Date.AddDays(1))),
                TableOperators.And,
                TableQuery.GenerateFilterCondition(nameof(BalanceOperationEntity.RowKey), QueryComparisons.LessThan,
                    GetPartitionKey(startDate.Date.AddMilliseconds(-1))));

            var query = new TableQuery<BalanceOperationEntity>().Where(filter).Take(limit);

            IEnumerable<BalanceOperationEntity> entities = await _storage.WhereAsync(query);

            return Mapper.Map<List<BalanceOperation>>(entities);
        }

        public async Task InsertAsync(BalanceOperation balanceOperation)
        {
            var entity = new BalanceOperationEntity(GetPartitionKey(balanceOperation.Time),
                GetRowKey(balanceOperation.Time));

            Mapper.Map(balanceOperation, entity);

            await _storage.InsertAsync(entity);
        }

        private static string GetPartitionKey(DateTime time)
            => (DateTime.MaxValue.Ticks - time.Date.Ticks).ToString("D19");

        private static string GetRowKey(DateTime time)
            => time.ToString("O");
    }
}
