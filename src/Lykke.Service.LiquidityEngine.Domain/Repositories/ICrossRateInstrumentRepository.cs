using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ICrossRateInstrumentRepository
    {
        Task<IReadOnlyCollection<CrossRateInstrument>> GetAllAsync();
        
        Task InsertAsync(CrossRateInstrument instrument);
        
        Task UpdateAsync(CrossRateInstrument instrument);
        
        Task DeleteAsync(string assetPairId);
    }
}
