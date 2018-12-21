using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using AzureStorage;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.AzureRepositories.Trades
{
    public class SettlementTradeRepository : ISettlementTradeRepository
    {
        private readonly INoSQLTableStorage<SettlementTradeEntity> _storage;

        public SettlementTradeRepository(INoSQLTableStorage<SettlementTradeEntity> storage)
        {
            _storage = storage;
        }

        public async Task<IReadOnlyCollection<SettlementTrade>> GetAllAsync()
        {
            IList<SettlementTradeEntity> entities = await _storage.GetDataAsync();

            return Mapper.Map<SettlementTrade[]>(entities);
        }

        public async Task<SettlementTrade> GetByIdAsync(string settlementTradeId)
        {
            SettlementTradeEntity entity = await _storage.GetDataAsync(GetPartitionKey(), GetRowKey(settlementTradeId));

            return Mapper.Map<SettlementTrade>(entity);
        }

        public async Task InsertAsync(SettlementTrade settlementTrade)
        {
            var entity = new SettlementTradeEntity(GetPartitionKey(), GetRowKey(settlementTrade.Id));

            Mapper.Map(settlementTrade, entity);

            await _storage.InsertAsync(entity);
        }

        public Task UpdateAsync(SettlementTrade settlementTrade)
        {
            return _storage.MergeAsync(GetPartitionKey(), GetRowKey(settlementTrade.Id), entity =>
            {
                Mapper.Map(settlementTrade, entity);
                return entity;
            });
        }

        private static string GetPartitionKey()
            => "SettlementTrade";

        private static string GetRowKey(string settlementTradeId)
            => settlementTradeId;
    }
}
