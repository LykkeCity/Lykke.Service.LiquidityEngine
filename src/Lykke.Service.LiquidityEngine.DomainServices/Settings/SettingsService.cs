using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Settings
{
    [UsedImplicitly]
    public class SettingsService : ISettingsService
    {
        private readonly string _instanceName;
        private readonly string _walletId;

        public SettingsService(string instanceName, string walletId)
        {
            _instanceName = instanceName;
            _walletId = walletId;
        }

        public Task<string> GetInstanceNameAsync()
        {
            return Task.FromResult(_instanceName);
        }

        public Task<string> GetWalletIdAsync()
        {
            return Task.FromResult(_walletId);
        }
    }
}
