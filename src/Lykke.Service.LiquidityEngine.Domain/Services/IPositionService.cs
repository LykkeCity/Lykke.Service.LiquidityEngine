using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IPositionService
    {
        Task<IReadOnlyCollection<Position>> GetAllAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId);

        Task<IReadOnlyCollection<Position>> GetOpenAllAsync();

        Task<IReadOnlyCollection<Position>> GetOpenByAssetPairIdAsync(string assetPairId);

        Task OpenAsync(IReadOnlyCollection<InternalTrade> internalTrades);

        Task CloseAsync(Position position, ExternalTrade externalTrade);

        Task CloseRemainingVolumeAsync(string assetPairId, ExternalTrade externalTrade);
    }
}
