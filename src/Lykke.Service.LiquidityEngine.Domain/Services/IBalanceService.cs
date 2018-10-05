using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IBalanceService
    {
        Task<IReadOnlyCollection<Balance>> GetAsync(string exchange);

        Task<Balance> GetByAssetIdAsync(string exchange, string assetId);

        Task UpdateLykkeBalancesAsync();

        Task UpdateExternalBalancesAsync();
    }
}
