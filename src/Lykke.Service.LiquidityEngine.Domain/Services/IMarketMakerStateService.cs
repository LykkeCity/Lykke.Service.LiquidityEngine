using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IMarketMakerStateService
    {
        Task<MarketMakerState> GetStateAsync();

        Task SetStateAsync(MarketMakerState state, string comment, string userId = null);
    }
}
