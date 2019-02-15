using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ILykkeExchangeService
    {
        Task ApplyAsync(string assetPairId, IReadOnlyCollection<LimitOrder> limitOrders);

        Task<string> CashInAsync(string clientId, string assetId, decimal amount, string userId, string comment);

        Task<string> CashOutAsync(string clientId, string assetId, decimal amount, string userId, string comment);

        Task<string> TransferAsync(string sourceWalletId, string destinationWalletId, string assetId, decimal amount,
            string transactionId = null);
    }
}
