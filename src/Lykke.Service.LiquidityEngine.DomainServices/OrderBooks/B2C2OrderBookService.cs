using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class B2C2OrderBookService : IB2C2OrderBookService
    {
        private readonly InMemoryCache<OrderBook> _cache;

        public B2C2OrderBookService()
        {
            _cache = new InMemoryCache<OrderBook>(orderBook => orderBook.AssetPairId, true);
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

            if (sellLimitOrders.Length != 2 || buyLimitOrders.Length != 2)
                return null;

            return new[]
            {
                new Quote(assetPairId, orderBook.Time, sellLimitOrders[0].Price, buyLimitOrders[0].Price,
                    ExchangeNames.B2C2),
                new Quote(assetPairId, orderBook.Time, sellLimitOrders[1].Price, buyLimitOrders[1].Price,
                    ExchangeNames.B2C2)
            };
        }
    }
}
