using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with asset pair mapping.
    /// </summary>
    [PublicAPI]
    public interface IAssetPairLinksApi
    {
        /// <summary>
        /// Returns a collection of asset pair links.
        /// </summary>
        /// <returns>A collection of asset pair links.</returns>
        [Get("/api/assetpairlinks")]
        Task<IReadOnlyCollection<AssetPairLinkModel>> GetAsync();

        /// <summary>
        /// Creates a new mapping between asset pairs.
        /// </summary>
        /// <param name="model">The model which describes mapping.</param>
        [Post("/api/assetpairlinks")]
        Task AddAsync([Body] AssetPairLinkModel model);
        
        /// <summary>
        /// Removes a new mapping between asset pairs.
        /// </summary>
        /// <param name="assetPairId">The internal asset pair id for which mapping should be removed.</param>
        [Delete("/api/assetpairlinks/{assetPairId}")]
        Task RemoveAsync(string assetPairId);
    }
}
