using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Timers
{
    [UsedImplicitly]
    public class MarketMakerTimer : Timer
    {
        private readonly IMarketMakerService _marketMakerService;
        private readonly ITimersSettingsService _timersSettingsService;
        private readonly IInstrumentService _instrumentService;

        public MarketMakerTimer(
            IMarketMakerService marketMakerService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory,
            IInstrumentService instrumentService)
        {
            _marketMakerService = marketMakerService;
            _timersSettingsService = timersSettingsService;
            _instrumentService = instrumentService;
            Log = logFactory.CreateLog(this);
        }
        
        protected override async Task OnExecuteAsync(CancellationToken cancellation)
        {
            var settings = (await _timersSettingsService.GetAsync()).MarketMaker;

            if (settings.TotalMilliseconds < 1)
                return;

            await _instrumentService.ResetTradingVolumeAsync();

            await _marketMakerService.UpdateOrderBooksAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            if (timersSettings.MarketMaker.TotalMilliseconds < 1)
                return TimeSpan.FromMinutes(1);

            return timersSettings.MarketMaker;
        }
    }
}
