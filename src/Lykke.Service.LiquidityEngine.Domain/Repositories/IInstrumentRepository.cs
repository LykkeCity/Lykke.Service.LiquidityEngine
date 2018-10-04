using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IInstrumentRepository
    {
        Task<IReadOnlyCollection<Instrument>> GetAllAsync();
        
        Task InsertAsync(Instrument instrument);
        
        Task UpdateAsync(Instrument instrument);
        
        Task DeleteAsync(string assetPairId);
    }
}
