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
    public class PnLStopLossTimer : Timer
    {
        private readonly IPnLStopLossEngineService _pnLStopLossEngineService;
        private readonly ITimersSettingsService _timersSettingsService;

        public PnLStopLossTimer(
            IPnLStopLossEngineService pnLStopLossEngineService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory)
        {
            _pnLStopLossEngineService = pnLStopLossEngineService;
            _timersSettingsService = timersSettingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _pnLStopLossEngineService.ExecuteAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return timersSettings.PnLStopLoss;
        }
    }
}
