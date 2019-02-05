using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossSettings;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with pnl stop losses.
    /// </summary>
    [PublicAPI]
    public interface IPnLStopLossSettingsApi
    {
        /// <summary>
        /// Returns a collection of pnl stop loss settings.
        /// </summary>
        /// <returns>Collection of pnl stop loss settings.</returns>
        [Get("/api/pnLStopLossSettings")]
        Task<IReadOnlyCollection<PnLStopLossSettingsModel>> GetAllAsync();

        /// <summary>
        /// Add new pnl stop loss settings.
        /// </summary>
        /// <param name="pnLStopLossSettingsModel">The model that describes pnl stop loss settings.</param>
        [Post("/api/pnLStopLossSettings")]
        Task AddAsync([Body] PnLStopLossSettingsModel pnLStopLossSettingsModel);

        /// <summary>
        /// Refreshes pnl stop loss settings.
        /// </summary>
        /// <param name="id">PnL stop loss settings id.</param>
        [Put("/api/pnLStopLossSettings/{id}/refresh")]
        Task RefreshAsync(string id);

        /// <summary>
        /// Deletes pnl stop loss settings by id.
        /// </summary>
        /// <param name="id">Identifier of the pnl stop loss settings.</param>
        [Delete("/api/pnLStopLossSettings/{id}")]
        Task DeleteAsync(string id);
    }
}
