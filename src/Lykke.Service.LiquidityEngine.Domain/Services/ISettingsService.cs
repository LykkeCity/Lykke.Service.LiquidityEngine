using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ISettingsService
    {
        Task<string> GetInstanceNameAsync();

        Task<string> GetWalletIdAsync();
    }
}
