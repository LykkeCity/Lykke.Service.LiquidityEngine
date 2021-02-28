﻿using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class B2C2OrderBookService : IB2C2OrderBookService
    {
        private readonly InMemoryCache<OrderBook> _cache;
        private readonly ILog _log;

        public B2C2OrderBookService(ILogFactory logFactory)
        {
            _cache = new InMemoryCache<OrderBook>(orderBook => orderBook.AssetPairId, true);

            _log = logFactory.CreateLog(this);
        }

        public Task SetAsync(OrderBook orderBook)
        {
            _cache.Set(orderBook);

            return Task.CompletedTask;
        }

        public Quote[] GetQuotes(string assetPairId)
        {
            OrderBook orderBook = _cache.Get(assetPairId);

            if (orderBook == null)
                return null;

            LimitOrder[] sellLimitOrders = orderBook.LimitOrders
                .Where(o => o.Type == LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .ToArray();

            LimitOrder[] buyLimitOrders = orderBook.LimitOrders
                .Where(o => o.Type == LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .ToArray();

            if (sellLimitOrders.Length == 0 || buyLimitOrders.Length == 0)
                return null;

            decimal secondAsk = sellLimitOrders[sellLimitOrders.Length-1].Price;

            decimal secondBid = buyLimitOrders[buyLimitOrders.Length-1].Price;

            return new[]
            {
                CreateQuote(orderBook, sellLimitOrders[0].Price, buyLimitOrders[0].Price),
                CreateQuote(orderBook, secondAsk, secondBid)
            };
        }

        private static Quote CreateQuote(OrderBook orderBook, decimal ask, decimal bid)
            => new Quote(orderBook.AssetPairId, orderBook.Time, ask, bid, ExchangeNames.B2C2);
    }
}
