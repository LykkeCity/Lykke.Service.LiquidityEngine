using System;
using System.Collections.Generic;
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
            _cache = new InMemoryCache<Quote>(GetKey, true);
        }

        public Task<IReadOnlyCollection<Quote>> GetAsync()
        {
            return Task.FromResult(_cache.GetAll());
        }

        public Task<Quote> GetAsync(string source, string assetPairId)
        {
            return Task.FromResult(_cache.Get($"{source}-{assetPairId}"));
        }

        public async Task SetAsync(Quote quote)
        {
            Quote currentQuote = _cache.Get(GetKey(quote));

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

        private static string GetKey(Quote quote)
            => GetKey(quote.Source, quote.AssetPair);

        private static string GetKey(string source, string assetPairId)
            => $"{source}-{assetPairId}";
    }
}
