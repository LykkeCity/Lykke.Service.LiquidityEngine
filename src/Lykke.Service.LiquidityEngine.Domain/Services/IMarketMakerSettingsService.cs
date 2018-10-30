using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IMarketMakerSettingsService
    {
        Task<MarketMakerSettings> GetAsync();

        Task UpdateAsync(MarketMakerSettings marketMakerSettings);
    }
}
