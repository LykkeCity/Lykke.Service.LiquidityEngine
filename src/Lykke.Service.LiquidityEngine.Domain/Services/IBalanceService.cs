using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IBalanceService
    {
        Task<IReadOnlyCollection<Balance>> GetAllAsync();

        Task<Balance> GetByAssetIdAsync(string assetId);

        Task UpdateAsync();
    }
}
