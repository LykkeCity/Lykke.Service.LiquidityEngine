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
    public class LykkeBalancesTimer : Timer
    {
        private readonly IBalanceService _balanceService;
        private readonly ITimersSettingsService _timersSettingsService;

        public LykkeBalancesTimer(
            IBalanceService balanceService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory)
        {
            _balanceService = balanceService;
            _timersSettingsService = timersSettingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return timersSettings.LykkeBalances;
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _balanceService.UpdateLykkeBalancesAsync();
        }
    }
}
