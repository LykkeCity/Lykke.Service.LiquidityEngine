using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settings
{
    [UsedImplicitly]
    public class MarketMakerSettingsService : IMarketMakerSettingsService
    {
        private const string CacheKey = "key";

        private readonly IMarketMakerSettingsRepository _marketMakerSettingsRepository;
        private readonly InMemoryCache<MarketMakerSettings> _cache;

        public MarketMakerSettingsService(IMarketMakerSettingsRepository marketMakerSettingsRepository)
        {
            _marketMakerSettingsRepository = marketMakerSettingsRepository;
            _cache = new InMemoryCache<MarketMakerSettings>(settings => CacheKey, true);
        }

        public async Task<MarketMakerSettings> GetAsync()
        {
            MarketMakerSettings marketMakerSettings = _cache.Get(CacheKey);

            if (marketMakerSettings == null)
            {
                marketMakerSettings = await _marketMakerSettingsRepository.GetAsync();

                if (marketMakerSettings == null)
                {
                    marketMakerSettings = new MarketMakerSettings
                    {
                        LimitOrderPriceMaxDeviation = 0.2m
                    };
                }

                _cache.Initialize(new[] {marketMakerSettings});
            }

            return marketMakerSettings;
        }

        public async Task UpdateAsync(MarketMakerSettings marketMakerSettings)
        {
            await _marketMakerSettingsRepository.InsertOrReplaceAsync(marketMakerSettings);

            _cache.Set(marketMakerSettings);
        }
    }
}
