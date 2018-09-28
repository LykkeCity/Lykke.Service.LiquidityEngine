using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Refit;

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
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions")]
        Task<IReadOnlyCollection<PositionModel>> GetAllAsync(DateTime startDate, DateTime endDate, int limit);

        /// <summary>
        /// Returns a collection of opened positions.
        /// </summary>
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions/open")]
        Task<IReadOnlyCollection<PositionModel>> GetOpenedAsync();
        
        /// <summary>
        /// Returns a collection of opened positions for asset pair.
        /// </summary>
        /// <param name="assetPairId">The asset pair id.</param>
        /// <returns>A collection of positions.</returns>
        [Get("/api/positions/open/{assetPairId}")]
        Task<IReadOnlyCollection<PositionModel>> GetOpenedByAssetPairIdAsync(string assetPairId);
    }
}
