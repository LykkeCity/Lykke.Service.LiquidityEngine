using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Sdk;
using Lykke.Service.LiquidityEngine.DomainServices.Timers;

namespace Lykke.Service.LiquidityEngine.Managers
{
    [UsedImplicitly]
    public class ShutdownManager : IShutdownManager
    {
        private readonly BalancesTimer _balancesTimer;

        public ShutdownManager(
            BalancesTimer balancesTimer)
        {
            _balancesTimer = balancesTimer;
        }
        
        public Task StopAsync()
        {
            _balancesTimer.Stop();
            
            return Task.CompletedTask;
        }
    }
}
