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
    public class SettlementsTimer : Timer
    {
        private readonly ISettlementTradeService _settlementTradeService;
        private readonly ITimersSettingsService _timersSettingsService;

        public SettlementsTimer(
            ISettlementTradeService settlementTradeService,
            ITimersSettingsService timersSettingsService,
            ILogFactory logFactory)
        {
            _settlementTradeService = settlementTradeService;
            _timersSettingsService = timersSettingsService;
            Log = logFactory.CreateLog(this);
        }

        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _settlementTradeService.FindTradesAsync();
        }

        protected override async Task<TimeSpan> GetDelayAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return timersSettings.Settlements;
        }
    }
}
