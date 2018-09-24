using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ILykkeExchangeService
    {
        Task<string> CashInAsync(string clientId, string assetId, decimal amount, string userId, string comment);

        Task<string> CashOutAsync(string clientId, string assetId, decimal amount, string userId, string comment);
    }
}
