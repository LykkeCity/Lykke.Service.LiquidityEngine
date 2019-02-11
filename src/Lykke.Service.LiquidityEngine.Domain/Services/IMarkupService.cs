using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IInstrumentMarkupService
    {
        Task<IReadOnlyCollection<AssetPairMarkup>> GetAllAsync();
    }
}
