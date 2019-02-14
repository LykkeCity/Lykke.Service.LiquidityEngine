using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface INoFreshQuotesStopLossService
    {
        Task<decimal> GetNoFreshQuotesMarkup(string assetPairId);

        Task<IReadOnlyCollection<string>> GetMessagesAsync(string assetPairId);

        Task<IReadOnlyCollection<AssetPairMarkup>> GetMarkupsAsync();
    }
}
