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
        private readonly MarketMakerTimer _marketMakerTimer;
        private readonly LykkeTradeSubscriber _lykkeTradeSubscriber;

        public StartupManager(
            BalancesTimer balancesTimer,
            MarketMakerTimer marketMakerTimer,
            LykkeTradeSubscriber lykkeTradeSubscriber)
        {
            _balancesTimer = balancesTimer;
            _marketMakerTimer = marketMakerTimer;
            _lykkeTradeSubscriber = lykkeTradeSubscriber;
        }

        public Task StartAsync()
        {
            _lykkeTradeSubscriber.Start();

            _balancesTimer.Start();
            
            _marketMakerTimer.Start();
            
            return Task.CompletedTask;
        }
    }
}
