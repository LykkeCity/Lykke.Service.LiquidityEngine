using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IMarketMakerStateService
    {
        Task<MarketMakerState> GetStateAsync();

        Task SetStateAsync(MarketMakerStatus status, string comment, string userId);

        Task SetStateAsync(MarketMakerError marketMakerError, string errorMessages, string comment);
    }
}
