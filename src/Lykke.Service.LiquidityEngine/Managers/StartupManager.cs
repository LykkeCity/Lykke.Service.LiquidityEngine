using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class StartupManager : IStartupManager
    {
        private readonly BalancesTimer _balancesTimer;

        public StartupManager(
            BalancesTimer balancesTimer)
        {
            _balancesTimer = balancesTimer;
        }

        public Task StartAsync()
        {
            _balancesTimer.Start();

            return Task.CompletedTask;
        }
    }
}
