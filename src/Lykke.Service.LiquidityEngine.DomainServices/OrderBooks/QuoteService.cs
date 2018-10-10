using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.OrderBooks
{
    [UsedImplicitly]
    public class QuoteService : IQuoteService
    {
        private readonly IQuoteThresholdSettingsService _quoteThresholdSettingsService;
        private readonly IQuoteThresholdLogService _quoteThresholdLogService;
        private readonly InMemoryCache<Quote> _cache;

        public QuoteService(
            IQuoteThresholdSettingsService quoteThresholdSettingsService,
            IQuoteThresholdLogService quoteThresholdLogService)
        {
            _quoteThresholdSettingsService = quoteThresholdSettingsService;
            _quoteThresholdLogService = quoteThresholdLogService;
            _cache = new InMemoryCache<Quote>(quote => quote.AssetPair, true);
        }

        public async Task SetAsync(Quote quote)
        {
            Quote currentQuote = _cache.Get(quote.AssetPair);

            if (currentQuote != null)
            {
                QuoteThresholdSettings quoteThresholdSettings = await _quoteThresholdSettingsService.GetAsync();

                if (quoteThresholdSettings.Enabled &&
                    Math.Abs(currentQuote.Mid - quote.Mid) / currentQuote.Mid > quoteThresholdSettings.Value)
                {
                    _quoteThresholdLogService.Error(currentQuote, quote, quoteThresholdSettings.Value);
                }
                else
                {
                    _cache.Set(quote);
                }
            }
            else
            {
                _cache.Set(quote);
            }
        }

        public Task<Quote> GetAsync(string assetPairId)
        {
            return Task.FromResult(_cache.Get(assetPairId));
        }
    }
}
