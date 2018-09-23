using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Balances;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with balances.
    /// </summary>
    [PublicAPI]
    public interface IBalancesApi
    {
        /// <summary>
        /// Returns a collection of non zero balances.
        /// </summary>
        /// <returns>A collection of balances.</returns>
        [Get("/api/balances")]
        Task<IReadOnlyCollection<AssetBalanceModel>> GetAllAsync();

        /// <summary>
        /// Returns a balance by asset.
        /// </summary>
        /// <param name="assetId">The identifier of an asset.</param>
        /// <returns>The balance of asset.</returns>
        [Get("/api/balances/{assetId}")]
        Task<AssetBalanceModel> GetByAssetIdAsync(string assetId);
    }
}
