using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;
using Lykke.Service.LiquidityEngine.Rabbit.Subscribers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly BalancesTimer _balancesTimer;
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;

        public StartupManager(
            BalancesTimer balancesTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber)
        {
            _balancesTimer = balancesTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
        }

        public Task StartAsync()
        {
            _lykkeTradeSubscriber.Start();

            _balancesTimer.Start();
            
            return Task.CompletedTask;
        }
    }
}
