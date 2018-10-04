using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Timers
{
    [UsedImplicitly]
    public class HedgingTimer : Timer
    {
        private readonly IHedgeService _hedgeService;

        public HedgingTimer(IHedgeService hedgeService)
        {
            _hedgeService = hedgeService;
        }
        
        protected override Task OnExecuteAsync(CancellationToken cancellation)
        {
            return _hedgeService.ExecuteAsync();
        }

        protected override Task<TimeSpan> GetDelayAsync()
        {
            return Task.FromResult(TimeSpan.FromSeconds(1));
        }
    }
}
