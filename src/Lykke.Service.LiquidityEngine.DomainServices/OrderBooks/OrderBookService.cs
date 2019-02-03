using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Publishers;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class OrderBookService : IOrderBookService
    {
        private readonly IInternalQuotePublisher _internalQuotePublisher;
        private readonly IInternalOrderBookPublisher _internalOrderBookPublisher;
        private readonly InMemoryCache<OrderBook> _cache;

        public OrderBookService(
            IInternalQuotePublisher internalQuotePublisher,
            IInternalOrderBookPublisher internalOrderBookPublisher)
        {
            _internalQuotePublisher = internalQuotePublisher;
            _internalOrderBookPublisher = internalOrderBookPublisher;
            _cache = new InMemoryCache<OrderBook>(orderBook => orderBook.AssetPairId, true);
        }

        public Task<IReadOnlyCollection<OrderBook>> GetAllAsync()
        {
            return Task.FromResult(_cache.GetAll());
        }

        public Task<OrderBook> GetByAssetPairIdAsync(string assetPairId)
        {
            return Task.FromResult(_cache.Get(assetPairId));
        }

        public async Task UpdateAsync(OrderBook orderBook)
        {
            _cache.Set(orderBook);

            LimitOrder sellLimitOrder = orderBook.LimitOrders
                .Where(o => o.Type == LimitOrderType.Sell)
                .OrderBy(o => o.Price)
                .FirstOrDefault();

            LimitOrder buyLimitOrder = orderBook.LimitOrders
                .Where(o => o.Type == LimitOrderType.Buy)
                .OrderByDescending(o => o.Price)
                .FirstOrDefault();

            if (sellLimitOrder != null && buyLimitOrder != null)
            {
                await _internalQuotePublisher.PublishAsync(new Quote(orderBook.AssetPairId, DateTime.UtcNow,
                    sellLimitOrder.Price, buyLimitOrder.Price, null));
            }

            await _internalOrderBookPublisher.PublishAsync(orderBook);
        }

        public Task RemoveAsync(string assetPairId)
        {
            _cache.Remove(assetPairId);

            return Task.CompletedTask;
        }
    }
}
