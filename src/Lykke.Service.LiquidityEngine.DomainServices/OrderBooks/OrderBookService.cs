using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class OrderBookService : IOrderBookService
    {
        private readonly InMemoryCache<OrderBook> _cache;
        
        public OrderBookService()
        {
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

        public Task UpdateAsync(OrderBook orderBook)
        {
            _cache.Set(orderBook);
            
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string assetPairId)
        {
            _cache.Remove(assetPairId);
            
            return Task.CompletedTask;
        }
    }
}
