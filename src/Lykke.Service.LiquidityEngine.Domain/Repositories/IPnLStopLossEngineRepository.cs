using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IPnLStopLossEngineRepository
    {
        Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllAsync();

        Task InsertAsync(PnLStopLossEngine pnLStopLossEngine);

        Task UpdateAsync(PnLStopLossEngine pnLStopLossEngine);

        Task DeleteAsync(string id);
    }
}
