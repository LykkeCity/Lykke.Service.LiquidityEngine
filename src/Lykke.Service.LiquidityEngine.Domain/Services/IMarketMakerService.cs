using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IMarketMakerService
    {
        Task UpdateOrderBooksAsync();
    }
}
