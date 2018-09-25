using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ISettlementService
    {
        Task ExecuteAsync(string assetId, decimal amount, string comment, string userId);
    }
}
