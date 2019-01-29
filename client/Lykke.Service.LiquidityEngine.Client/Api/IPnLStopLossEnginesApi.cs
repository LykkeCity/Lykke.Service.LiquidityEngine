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
        /// Updates pnl stop loss engine.
        /// </summary>
        /// <param name="pnLStopLossEngineModel"></param>
        /// <returns></returns>
        [Put("/api/pnLStopLossEngines/{id}")]
        Task UpdateAsync(PnLStopLossEngineModel pnLStopLossEngineModel);

        /// <summary>
        /// Deletes pnl stop loss engine by id.
        /// </summary>
        /// <param name="id">Identifier.</param>
        [Delete("/api/pnLStopLossEngines/{id}")]
        Task DeleteAsync(string id);
    }
}
