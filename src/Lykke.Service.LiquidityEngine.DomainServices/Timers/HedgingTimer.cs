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
    public class HedgingTimer : Timer
    {
        private readonly IHedgeService _hedgeService;
        private readonly ITimersSettingsService _timersSettingsService;

        public HedgingTimer(
            IHedgeService hedgeService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory)
        {
            _hedgeService = hedgeService;
            _timersSettingsService = timersSettingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _hedgeService.ExecuteAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return timersSettings.Hedging;
        }
    }
}
