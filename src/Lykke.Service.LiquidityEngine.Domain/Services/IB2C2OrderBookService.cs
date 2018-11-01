using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IB2C2OrderBookService
    {
        Task SetAsync(OrderBook orderBook);
        
        Quote[] GetQuotes(string assetPairId);
    }
}
