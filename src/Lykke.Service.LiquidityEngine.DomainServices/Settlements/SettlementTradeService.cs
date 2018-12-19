using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Lykke.Service.Dwh.Client;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settlements
{
    public class SettlementTradeService : ISettlementTradeService
    {
        private readonly ISettlementTradeRepository _settlementTradeRepository;
        private readonly IDwhClient _dwhClient;
        private readonly string _database;
        private readonly string _tradesSpName;

        private readonly InMemoryCache<SettlementTrade> _cache;

        public SettlementTradeService(
            ISettlementTradeRepository settlementTradeRepository,
            IDwhClient dwhClient,
            string database,
            string tradesSpName)
        {
            _settlementTradeRepository = settlementTradeRepository;
            _dwhClient = dwhClient;
            _database = database;
            _tradesSpName = tradesSpName;

            _cache = new InMemoryCache<SettlementTrade>(GetKey, false);
        }

        public async Task<IReadOnlyCollection<SettlementTrade>> GetAllAsync()
        {
            IReadOnlyCollection<SettlementTrade> settlementTrades = _cache.GetAll();

            if (settlementTrades == null)
            {
                settlementTrades = await _settlementTradeRepository.GetAllAsync();

                _cache.Initialize(settlementTrades);
            }

            return settlementTrades;
        }

        public async Task<SettlementTrade> GetByIdAsync(string settlementTradeId)
        {
            IReadOnlyCollection<SettlementTrade> settlementTrades = await GetAllAsync();

            return settlementTrades.SingleOrDefault(o => o.Id == settlementTradeId);
        }

        public async Task UpdateAsync(SettlementTrade settlementTrade)
        {
            await _settlementTradeRepository.UpdateAsync(settlementTrade);
            
            _cache.Set(settlementTrade);
        }

        public async Task FindTradesAsync()
        {
            IReadOnlyCollection<SettlementTrade> settlementTrades = await GetTradesAsync();

            IReadOnlyCollection<SettlementTrade> existingSettlementTrades = await GetAllAsync();

            foreach (SettlementTrade settlementTrade in settlementTrades)
            {
                if (existingSettlementTrades.Any(o => o.Id == settlementTrade.Id))
                    continue;

                await _settlementTradeRepository.InsertAsync(settlementTrade);

                _cache.Set(settlementTrade);
            }
        }

        private async Task<IReadOnlyCollection<SettlementTrade>> GetTradesAsync()
        {
            ResponceDataSet response = await RetriesWrapper.RunWithRetriesAsync(() =>
                _dwhClient.Call(new Dictionary<string, string>
                    {
                        ["fromTime"] = "2018-01-01"
                    },
                    _tradesSpName, _database));

            var settlementTrades = new List<SettlementTrade>();

            if (response.Data == null)
                return settlementTrades;

            List<Trade> trades;
            List<Transaction> transactions;

            using (DataTableReader reader = response.Data.Tables[0].CreateDataReader())
                trades = Mapper.Map<List<Trade>>(reader);

            using (DataTableReader reader = response.Data.Tables[1].CreateDataReader())
                transactions = Mapper.Map<List<Transaction>>(reader);

            foreach (Trade trade in trades)
            {
                TradeType tradeType;

                if (trade.Direction == "sell")
                    tradeType = TradeType.Sell;
                else if (trade.Direction == "buy")
                    tradeType = TradeType.Buy;
                else
                    continue;

                Transaction[] tradeTransactions = transactions.Where(o => o.TradeId == trade.TradeId).ToArray();

                if (tradeTransactions.Length != 2)
                    continue;

                if (tradeTransactions.All(o => !trade.AssetPair.StartsWith(o.Asset)) &&
                    tradeTransactions.All(o => !trade.AssetPair.EndsWith(o.Asset)))
                    continue;

                var settlementTrade = new SettlementTrade
                {
                    Id = trade.TradeId,
                    Type = tradeType,
                    AssetPair = trade.AssetPair,
                    Price = (decimal) trade.Price,
                    Timestamp = trade.Timestamp,
                    IsCompleted = false
                };

                foreach (Transaction transaction in tradeTransactions)
                {
                    if (settlementTrade.AssetPair.StartsWith(transaction.Asset))
                    {
                        settlementTrade.BaseAsset = transaction.Asset;
                        settlementTrade.Volume = (decimal) Math.Abs(transaction.Amount);
                    }
                    else if (settlementTrade.AssetPair.EndsWith(transaction.Asset))
                    {
                        settlementTrade.QuoteAsset = transaction.Asset;
                        settlementTrade.OppositeVolume = (decimal) Math.Abs(transaction.Amount);
                    }
                }

                settlementTrades.Add(settlementTrade);
            }

            return settlementTrades;
        }

        private static string GetKey(SettlementTrade settlementTrade)
            => GetKey(settlementTrade.Id);

        private static string GetKey(string settlementTradeId)
            => settlementTradeId.ToUpper();

        #region Nested classes

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class Trade
        {
            public DateTime Timestamp { get; set; }

            public string TradeId { get; set; }

            public string Direction { get; set; }

            public double Price { get; set; }

            public string AssetPair { get; set; }
        }

        [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
        public class Transaction
        {
            public DateTime Timestamp { get; set; }

            public string TransactionId { get; set; }

            public string TradeId { get; set; }

            public string Asset { get; set; }

            public double Amount { get; set; }
        }

        #endregion
    }
}
