using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class QuoteService : IQuoteService
    {
        private readonly InMemoryCache<Quote> _cache;
        
        public QuoteService()
        {
            _cache = new InMemoryCache<Quote>(quote => quote.AssetPair, true);
        }
        
        public Task SetAsync(Quote quote)
        {
            _cache.Set(quote);
            
            return Task.CompletedTask;
        }

        public Task<Quote> GetAsync(string assetPairId)
        {
            return Task.FromResult(_cache.Get(assetPairId));
        }
    }
}
