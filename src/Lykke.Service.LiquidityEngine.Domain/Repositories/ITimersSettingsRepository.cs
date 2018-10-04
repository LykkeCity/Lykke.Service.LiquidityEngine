using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ITimersSettingsRepository
    {
        Task<TimersSettings> GetAsync();
        
        Task InsertOrReplaceAsync(TimersSettings timersSettings);
    }
}
