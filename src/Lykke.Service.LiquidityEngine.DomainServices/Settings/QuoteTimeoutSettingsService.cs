using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settings
{
    [UsedImplicitly]
    public class QuoteTimeoutSettingsService : IQuoteTimeoutSettingsService
    {
        private const string CacheKey = "key";

        private readonly IQuoteTimeoutSettingsRepository _quoteTimeoutSettingsRepository;
        private readonly InMemoryCache<QuoteTimeoutSettings> _cache;

        public QuoteTimeoutSettingsService(IQuoteTimeoutSettingsRepository quoteTimeoutSettingsRepository)
        {
            _quoteTimeoutSettingsRepository = quoteTimeoutSettingsRepository;
            _cache = new InMemoryCache<QuoteTimeoutSettings>(settings => CacheKey, false);
        }

        public async Task<QuoteTimeoutSettings> GetAsync()
        {
            QuoteTimeoutSettings quoteTimeoutSettings = _cache.Get(CacheKey);

            if (quoteTimeoutSettings == null)
            {
                quoteTimeoutSettings = await _quoteTimeoutSettingsRepository.GetAsync();

                if (quoteTimeoutSettings == null)
                {
                    quoteTimeoutSettings = new QuoteTimeoutSettings
                    {
                        Enabled = true,
                        Error = TimeSpan.FromSeconds(5)
                    };
                }

                _cache.Initialize(new[] {quoteTimeoutSettings});
            }

            return quoteTimeoutSettings;
        }

        public async Task SaveAsync(QuoteTimeoutSettings quoteTimeoutSettings)
        {
            await _quoteTimeoutSettingsRepository.InsertOrReplaceAsync(quoteTimeoutSettings);

            _cache.Set(quoteTimeoutSettings);
        }
    }
}
