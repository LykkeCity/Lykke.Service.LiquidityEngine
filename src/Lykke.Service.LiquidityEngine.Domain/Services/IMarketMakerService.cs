using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IMarketMakerService
    {
        void Start();

        void Stop();

        Task UpdateOrderBooksAsync(string assetPairId = null);
    }
}
