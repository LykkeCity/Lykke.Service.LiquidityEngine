using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly BalancesTimer _balancesTimer;
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;

        public ShutdownManager(
            BalancesTimer balancesTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber)
        {
            _balancesTimer = balancesTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
        }
        
        public Task StopAsync()
        {
            _balancesTimer.Stop();
            
            _lykkeTradeSubscriber.Stop();
            
            return Task.CompletedTask;
        }
    }
}
