using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.MarketMaker;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with market maker state.
    /// </summary>
    [PublicAPI]
    public interface IMarketMakerApi
    {
        /// <summary>
        /// Returns current market maker state
        /// </summary>
        /// <returns>Market maker state</returns>
        [Get("/api/marketmaker/state")]
        Task<MarketMakerStateModel> GetStateAsync();

        /// <summary>
        /// Sets market maker state to the specified value
        /// </summary>
        /// <param name="model">Market maker state to set</param>
        [Post("/api/marketmaker/state")]
        Task SetStateAsync(MarketMakerStateUpdateModel model);
    }
}
