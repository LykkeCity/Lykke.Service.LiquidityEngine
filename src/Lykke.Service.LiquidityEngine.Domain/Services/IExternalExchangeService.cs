using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IExternalExchangeService
    {
        Task<IReadOnlyCollection<Balance>> GetBalancesAsync();

        Task<ExternalTrade> ExecuteSellLimitOrderAsync(string assetPairId, decimal volume);

        Task<ExternalTrade> ExecuteBuyLimitOrderAsync(string assetPairId, decimal volume);

        Task<ExternalTrade> ExecuteLimitOrderAsync(string assetPairId, decimal volume, decimal price,
            LimitOrderType limitOrderType);
    }
}
