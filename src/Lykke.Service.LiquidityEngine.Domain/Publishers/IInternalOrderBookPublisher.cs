using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Publishers
{
    public interface IInternalOrderBookPublisher
    {
        Task PublishAsync(OrderBook orderBook);
    }
}
