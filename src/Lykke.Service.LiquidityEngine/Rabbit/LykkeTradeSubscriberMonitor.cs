using System;
using System.Threading;
using System.Threading.Tasks;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;
using Timer = Lykke.Service.LiquidityEngine.DomainServices.Timers.Timer;

namespace Lykke.Service.LiquidityEngine.Rabbit
{
    public class LykkeTradeSubscriberMonitor : Timer
    {
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;

        public LykkeTradeSubscriberMonitor(
            LykkeTradeSubscriber lykkeTradeSubscriber,
            ILogFactory logFactory)
        {
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
            Log = logFactory.CreateLog(this);
        }

        protected override async Task OnExecuteAsync(CancellationToken cancellation)
        {
            if (DateTime.UtcNow - _lykkeTradeSubscriber.LastMessageTime > TimeSpan.FromMinutes(15))
            {
                Log.WarningWithDetails("No trades received for five minutes. Subscriber will be reconnected.", new
                {
                    _lykkeTradeSubscriber.LastMessageTime
                });
                
                _lykkeTradeSubscriber.Stop();

                await Task.Delay(TimeSpan.FromMinutes(1), cancellation);

                if (!cancellation.IsCancellationRequested)
                {
                    _lykkeTradeSubscriber.Start();
                }
            }
        }

        protected override Task<TimeSpan> GetDelayAsync()
        {
            return Task.FromResult(TimeSpan.FromSeconds(1));
        }
    }
}
