using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.AssetSettings;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with asset settings.
    /// </summary>
    [PublicAPI]
    public interface IAssetSettingsApi
    {
        /// <summary>
        /// Returns a collection of asset settings.
        /// </summary>
        /// <returns>A collection of asset settings.</returns>
        [Get("/api/assetsettings")]
        Task<IReadOnlyCollection<AssetSettingsModel>> GetAllAsync();

        /// <summary>
        /// Returns settings for the specified asset by asset id.
        /// </summary>
        /// <param name="assetId">The asses id.</param>
        /// <returns>Asset settings.</returns>
        [Get("/api/assetsettings/{assetId}")]
        Task<AssetSettingsModel> GetByIdAsync(string assetId);

        /// <summary>
        /// Adds new settings for an asset.
        /// </summary>
        /// <param name="assetSettingsModel">The model which describes asset settings.</param>
        [Post("/api/assetsettings")]
        Task AddAsync([Body] AssetSettingsModel assetSettingsModel);

        /// <summary>
        /// Updates asset settings.
        /// </summary>
        /// <param name="assetSettingsModel">The model which describes asset settings.</param>
        [Put("/api/assetsettings")]
        Task UpdateAsync([Body] AssetSettingsModel assetSettingsModel);

        /// <summary>
        /// Deletes the asset settings by asset id.
        /// </summary>
        /// <param name="assetId">The asses id.</param>
        [Delete("/api/assetsettings/{assetId}")]
        Task DeleteAsync(string assetId);
    }
}
