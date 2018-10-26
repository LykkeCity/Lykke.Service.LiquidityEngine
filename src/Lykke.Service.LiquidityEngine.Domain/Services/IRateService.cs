using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IRateService
    {
        Task<decimal> GetQuotingToUsdRate(string assetPairId);
    }
}
