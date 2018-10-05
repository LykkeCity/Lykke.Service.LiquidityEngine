using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IExternalExchangeService
    {
        Task<IReadOnlyCollection<Balance>> GetBalancesAsync();

        Task<decimal> GetSellPriceAsync(string assetPair, decimal volume);
        
        Task<decimal> GetBuyPriceAsync(string assetPair, decimal volume);

        Task<ExternalTrade> ExecuteSellLimitOrderAsync(string assetPairId, decimal volume);

        Task<ExternalTrade> ExecuteBuyLimitOrderAsync(string assetPairId, decimal volume);
    }
}
