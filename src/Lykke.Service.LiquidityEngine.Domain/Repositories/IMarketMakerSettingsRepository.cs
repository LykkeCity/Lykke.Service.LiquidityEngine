using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IMarketMakerSettingsRepository
    {
        Task<MarketMakerSettings> GetAsync();

        Task InsertOrReplaceAsync(MarketMakerSettings marketMakerSettings);
    }
}
