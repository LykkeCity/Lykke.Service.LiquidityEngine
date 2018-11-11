using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface ICrossInstrumentRepository
    {
        Task<IReadOnlyCollection<CrossInstrument>> GetAsync(string assetPairId);

        Task AddAsync(string assetPairId, CrossInstrument crossInstrument);

        Task UpdateAsync(string assetPairId, CrossInstrument crossInstrument);

        Task DeleteAsync(string assetPairId);

        Task DeleteAsync(string assetPairId, string crossAssetPairId);
    }
}
