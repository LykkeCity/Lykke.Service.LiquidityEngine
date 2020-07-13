using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;

namespace Lykke.Service.LiquidityEngine.Domain.Publishers
{
    public interface IOrderBooksUpdatesReportPublisher
    {
        Task PublishAsync(OrderBookUpdateReport orderBookUpdateReport);
    }
}
