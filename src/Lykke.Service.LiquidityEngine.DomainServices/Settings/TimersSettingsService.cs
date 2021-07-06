﻿using System;
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
            _cache = new InMemoryCache<TimersSettings>(settings => CacheKey, false);
        }

        public async Task<TimersSettings> GetAsync()
        {
            TimersSettings timersSettings = _cache.Get(CacheKey);

            if (timersSettings == null)
            {
                timersSettings = await _timersSettingsRepository.GetAsync() ?? new TimersSettings();

                if (timersSettings.MarketMaker == TimeSpan.Zero)
                    timersSettings.MarketMaker = TimeSpan.FromSeconds(5);

                if (timersSettings.Hedging == TimeSpan.Zero)
                    timersSettings.Hedging = TimeSpan.FromSeconds(1);

                if (timersSettings.LykkeBalances == TimeSpan.Zero)
                    timersSettings.LykkeBalances = TimeSpan.FromSeconds(1);

                if (timersSettings.ExternalBalances == TimeSpan.Zero)
                    timersSettings.ExternalBalances = TimeSpan.FromSeconds(1);

                if (timersSettings.Settlements == TimeSpan.Zero)
                    timersSettings.Settlements = TimeSpan.FromSeconds(5);

                if (timersSettings.InternalTrader == TimeSpan.Zero)
                    timersSettings.InternalTrader = TimeSpan.FromSeconds(5);

                if (timersSettings.PnLStopLoss == TimeSpan.Zero)
                    timersSettings.PnLStopLoss = TimeSpan.FromSeconds(1);

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
