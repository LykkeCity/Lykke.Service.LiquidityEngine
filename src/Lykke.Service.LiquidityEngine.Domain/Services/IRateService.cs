using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IRateService
    {
        Task<decimal?> CalculatePriceInUsd(string assetPairId, decimal price);
    }
}
