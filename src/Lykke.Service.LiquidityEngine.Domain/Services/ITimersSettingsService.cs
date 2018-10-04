using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ITimersSettingsService
    {
        Task<TimersSettings> GetAsync();

        Task SaveAsync(TimersSettings timersSettings);
    }
}
