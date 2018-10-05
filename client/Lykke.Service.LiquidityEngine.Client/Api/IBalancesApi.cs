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
        /// Returns a collection of non zero balances for Lykke exchange
        /// </summary>
        /// <returns>A collection of balances.</returns>
        [Get("/api/balances/lykke")]
        Task<IReadOnlyCollection<AssetBalanceModel>> GetLykkeAsync();

        /// <summary>
        /// Returns a balance for Lykke exchange by asset.
        /// </summary>
        /// <param name="assetId">The identifier of an asset.</param>
        /// <returns>The balance of asset.</returns>
        [Get("/api/balances/lykke/{assetId}")]
        Task<AssetBalanceModel> GetLykkeBalanceByAssetIdAsync(string assetId);

        /// <summary>
        /// Returns a collection of non zero balances for External exchange
        /// </summary>
        /// <returns>A collection of balances.</returns>
        [Get("/api/balances/external")]
        Task<IReadOnlyCollection<AssetBalanceModel>> GetExternalAsync();
    }
}
