using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ICrossRateInstrumentService
    {
        Task<IReadOnlyCollection<CrossRateInstrument>> GetAllAsync();
        
        Task<CrossRateInstrument> GetByAssetPairIdAsync(string assetPairId);
        
        Task AddAsync(CrossRateInstrument crossInstrument);
        
        Task UpdateAsync(CrossRateInstrument crossInstrument);
        
        Task DeleteAsync(string assetPairId);

        Task<decimal?> ConvertPriceAsync(string assetPairId, decimal price);
    }
}
