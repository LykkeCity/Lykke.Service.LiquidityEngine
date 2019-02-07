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
    public class InternalTraderTimer : Timer
    {
        private readonly IInternalTraderService _internalTraderService;
        private readonly ITimersSettingsService _timersSettingsService;

        public InternalTraderTimer(
            IInternalTraderService internalTraderService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory)
        {
            _internalTraderService = internalTraderService;
            _timersSettingsService = timersSettingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _internalTraderService.ExecuteAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return timersSettings.InternalTrader;
        }
    }
}
