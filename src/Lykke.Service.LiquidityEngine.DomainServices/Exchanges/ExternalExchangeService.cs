using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Exchanges
{
    public class ExternalExchangeService : IExternalExchangeService
    {
        public Task<decimal> GetSellPriceAsync(string assetPair, decimal volume)
        {
            throw new System.NotImplementedException();
        }

        public Task<decimal> GetBuyPriceAsync(string assetPair, decimal volume)
        {
            throw new System.NotImplementedException();
        }

        public Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume)
        {
            throw new System.NotImplementedException();
        }
    }
}
