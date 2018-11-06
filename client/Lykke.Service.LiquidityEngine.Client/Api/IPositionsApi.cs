using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with position.
    /// </summary>
    [PublicAPI]
    public interface IPositionsApi
    {
        /// <summary>
        /// Returns a collection of positions for the period.
        /// </summary>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The maximum number of positions.</param>
        /// <param name="assetPairId">Asset pair identifier to filter positions by asset pair</param>
        /// <param name="tradeAssetPairId">Asset pair identifir to filter positions by traded asset pair</param>
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions")]
        Task<IReadOnlyCollection<PositionModel>> GetAllAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId);

        /// <summary>
        /// Returns a collection of opened positions.
        /// </summary>
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions/open")]
        Task<IReadOnlyCollection<PositionModel>> GetOpenAllAsync();

        /// <summary>
        /// Returns a collection of opened positions for asset pair.
        /// </summary>
        /// <param name="assetPairId">The asset pair id.</param>
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions/open/{assetPairId}")]
        Task<IReadOnlyCollection<PositionModel>> GetOpenByAssetPairIdAsync(string assetPairId);
    }
}
