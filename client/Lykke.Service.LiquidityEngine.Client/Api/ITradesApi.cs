using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Trades;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with trades.
    /// </summary>
    [PublicAPI]
    public interface ITradesApi
    {
        /// <summary>
        /// Returns a collection of external trades for the period.
        /// </summary>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The maximum number of trades.</param>
        /// <returns>A collection of external trades.</returns>
        [Get("/api/trades/external")]
        Task<IReadOnlyCollection<ExternalTradeModel>> GetExternalTradesAsync(DateTime startDate, DateTime endDate,
            int limit);

        /// <summary>
        /// Returns an external trade by id.
        /// </summary>
        /// <param name="tradeId">The trade id.</param>
        /// <returns>An external trade.</returns>
        [Get("/api/trades/external/{tradeId}")]
        Task<ExternalTradeModel> GetExternalTradeByIdAsync(string tradeId);

        /// <summary>
        /// Returns a collection of internal trades for the period.
        /// </summary>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The maximum number of trades.</param>
        /// <returns>A collection of internal trades.</returns>
        [Get("/api/trades/internal")]
        Task<IReadOnlyCollection<InternalTradeModel>> GetInternalTradesAsync(DateTime startDate, DateTime endDate,
            int limit);

        /// <summary>
        /// Returns an internal trade by id.
        /// </summary>
        /// <param name="tradeId">The trade id.</param>
        /// <returns>An internal trade.</returns>
        [Get("/api/trades/internal/{tradeId}")]
        Task<InternalTradeModel> GetInternalTradeByIdAsync(string tradeId);

        /// <summary>
        /// Returns last internal trade's time for the specified asset pair.
        /// </summary>
        /// <param name="assetPairId">The asset pair identifier.</param>
        /// <returns>Last internal trade time.</returns>
        [Get("/api/trades/internal/{assetPairId}/time")]
        Task<LastInternalTradeTimeModel> GetLastInternalTradeTimeAsync(string assetPairId);

        /// <summary>
        /// Returns a collection of B2C2 settlement trades.
        /// </summary>
        /// <returns>A collection of settlement trades.</returns>
        [Get("/api/trades/settlement")]
        Task<IReadOnlyCollection<SettlementTradeModel>> GetSettlementTradesAsync();
    }
}
