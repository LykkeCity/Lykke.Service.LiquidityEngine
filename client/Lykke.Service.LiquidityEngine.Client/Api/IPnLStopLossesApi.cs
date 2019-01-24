using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLosses;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with pnl stop losses.
    /// </summary>
    [PublicAPI]
    public interface IPnLStopLossesApi
    {
        /// <summary>
        /// Returns a collection of pnl stop loss engines.
        /// </summary>
        /// <returns>Collection of pnl stop loss engines.</returns>
        [Get("/api/pnLStopLosses/engines")]
        Task<IReadOnlyCollection<PnLStopLossEngineModel>> GetAllEnginesAsync();

        /// <summary>
        /// Creates one or many (for all asset pairs) pnl stop loss engines.
        /// One is for particular asset pair and
        /// many is for all asset pairs if settings are global.
        /// </summary>
        /// <param name="pnLStopLossSettingsModel">The model that describes pnl stop loss settings.</param>
        [Post("/api/pnLStopLosses/create")]
        Task CreateAsync([Body] PnLStopLossSettingsModel pnLStopLossSettingsModel);

        /// <summary>
        /// Deletes pnl stop loss engine by id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        [Delete("/api/pnLStopLosses/engines/{id}")]
        Task DeleteEngineAsync(string id);

        /// <summary>
        /// Returns a collection of pnl stop loss global settings.
        /// </summary>
        /// <returns>Collection of pnl stop loss global settings.</returns>
        [Get("/api/pnLStopLosses/globalSettings")]
        Task<IReadOnlyCollection<PnLStopLossSettingsModel>> GetAllGlobalSettingsAsync();

        /// <summary>
        /// Reapplies global settings to asset pairs where they were deleted.
        /// </summary>
        /// <param name="id">Global settings id.</param>
        [Put("/api/pnLStopLosses/reapplyGlobalSettings/{id}")]
        Task ReapplyGlobalSettingsAsync(string id);

        /// <summary>
        /// Deletes pnl stop loss settings by id.
        /// </summary>
        /// <param name="id">Identifier of the settings.</param>
        [Delete("/api/pnLStopLosses/settings/{id}")]
        Task DeleteGlobalSettingsAsync(string id);

        /// <summary>
        /// Updates engine mode to a new value.
        /// </summary>
        /// <param name="id">PnL stop loss engine.</param>
        /// <param name="mode">New mode.</param>
        /// <returns></returns>
        [Put("/api/pnLStopLosses/update/{id}/{mode}")]
        Task UpdateEngineModeAsync(string id, PnLStopLossEngineMode mode);
    }
}
