using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.Reports.OrderBookUpdates;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IOrderBookUpdateReportRepository
    {
        Task InsertAsync(OrderBookUpdateReport orderBookUpdateReport);
    }
}
