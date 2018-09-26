using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IOpenPositionRepository
    {
        Task<IReadOnlyCollection<Position>> GetAllAsync();

        Task<IReadOnlyCollection<Position>> GetByAssetPairIdAsync(string assetPairId);

        Task InsertAsync(Position position);

        Task DeleteAsync(string assetPairId, string positionId);
    }
}
