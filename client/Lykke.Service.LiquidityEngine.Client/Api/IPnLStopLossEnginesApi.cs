using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with pnl stop loss engines.
    /// </summary>
    [PublicAPI]
    public interface IPnLStopLossEnginesApi
    {
        /// <summary>
        /// Returns a collection of pnl stop loss engines.
        /// </summary>
        /// <returns>Collection of pnl stop loss engines.</returns>
        [Get("/api/pnLStopLossEngines")]
        Task<IReadOnlyCollection<PnLStopLossEngineModel>> GetAllAsync();

        /// <summary>
        /// Add new pnl stop loss engine.
        /// </summary>
        /// <param name="pnLStopLossEngineModel">The model that describes pnl stop loss engine.</param>
        [Post("/api/pnLStopLossEngines")]
        Task AddAsync([Body] PnLStopLossEngineModel pnLStopLossEngineModel);

        /// <summary>
        /// Updates pnl stop loss engine.
        /// </summary>
        /// <param name="pnLStopLossEngineModel"></param>
        [Put("/api/pnLStopLossEngines")]
        Task UpdateAsync([Body] PnLStopLossEngineModel pnLStopLossEngineModel);

        /// <summary>
        /// Disable pnl stop loss engine.
        /// </summary>
        /// <param name="id">PnL stop loss engine id.</param>
        [Put("/api/pnLStopLossEngines/disable/{id}")]
        Task DisableAsync(string id);

        /// <summary>
        /// Enable pnl stop loss engine.
        /// </summary>
        /// <param name="id">PnL stop loss engine id.</param>
        [Put("/api/pnLStopLossEngines/enable/{id}")]
        Task EnableAsync(string id);

        /// <summary>
        /// Deletes pnl stop loss engine by id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        [Delete("/api/pnLStopLossEngines/{id}")]
        Task DeleteAsync(string id);

        /// <summary>
        /// Returns a collection of total markups for every asset pair.
        /// </summary>
        /// <returns>Collection of total markups for every asset pair.</returns>
        [Get("/api/pnLStopLossEngines/assetPairMarkups")]
        Task<IReadOnlyCollection<AssetPairMarkupModel>> GetAssetPairMarkupsAsync();
    }
}
