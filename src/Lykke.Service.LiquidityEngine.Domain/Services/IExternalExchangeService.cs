using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IExternalExchangeService
    {
        Task<decimal> GetSellPriceAsync(string assetPair, decimal volume);
        
        Task<decimal> GetBuyPriceAsync(string assetPair, decimal volume);

        Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume);
    }
}
