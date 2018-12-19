using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settings
{
    [UsedImplicitly]
    public class TimersSettingsService : ITimersSettingsService
    {
        private const string CacheKey = "key";

        private readonly ITimersSettingsRepository _timersSettingsRepository;
        private readonly InMemoryCache<TimersSettings> _cache;

        public TimersSettingsService(ITimersSettingsRepository timersSettingsRepository)
        {
            _timersSettingsRepository = timersSettingsRepository;
            _cache = new InMemoryCache<TimersSettings>(settings => CacheKey, true);
        }

        public async Task<TimersSettings> GetAsync()
        {
            TimersSettings timersSettings = _cache.Get(CacheKey);

            if (timersSettings == null)
            {
                timersSettings = await _timersSettingsRepository.GetAsync();

                if (timersSettings == null)
                {
                    timersSettings = new TimersSettings
                    {
                        MarketMaker = TimeSpan.FromSeconds(5),
                        Hedging = TimeSpan.FromSeconds(1),
                        LykkeBalances = TimeSpan.FromSeconds(1),
                        ExternalBalances = TimeSpan.FromSeconds(1),
                        Settlements = TimeSpan.FromSeconds(5)
                    };
                }

                _cache.Initialize(new[] {timersSettings});
            }

            return timersSettings;
        }

        public async Task SaveAsync(TimersSettings timersSettings)
        {
            await _timersSettingsRepository.InsertOrReplaceAsync(timersSettings);

            _cache.Set(timersSettings);
        }
    }
}
