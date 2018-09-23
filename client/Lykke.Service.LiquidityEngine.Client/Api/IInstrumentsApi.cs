using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with instruments.
    /// </summary>
    [PublicAPI]
    public interface IInstrumentsApi
    {
        /// <summary>
        /// Returns a collection of instruments.
        /// </summary>
        /// <returns>A collection of instruments.</returns>
        [Get("/api/instruments")]
        Task<IReadOnlyCollection<InstrumentModel>> GetAllAsync();

        /// <summary>
        /// Returns an instrument by asset pair id.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        /// <returns>An instrument.</returns>
        [Get("/api/instruments/{assetPairId}")]
        Task<InstrumentModel> GetByAssetPairIdAsync(string assetPairId);

        /// <summary>
        /// Adds new instrument settings (without levels).
        /// </summary>
        /// <param name="model">The model which describes instrument.</param>
        [Post("/api/instruments")]
        Task AddAsync([Body] InstrumentModel model);

        /// <summary>
        /// Updates instrument settings (without levels).
        /// </summary>
        /// <param name="model">The model which describes instrument.</param>
        [Put("/api/instruments")]
        Task UpdateAsync([Body] InstrumentModel model);

        /// <summary>
        /// Deletes the instrument settings by asset pair id.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        [Delete("/api/instruments/{assetPairId}")]
        Task DeleteAsync(string assetPairId);

        /// <summary>
        /// Adds volume level for the instrument.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        /// <param name="model">The model which describes level volume.</param>
        [Post("/api/instruments/{assetPairId}/levels")]
        Task AddLevelAsync(string assetPairId, [Body] LevelVolumeModel model);

        /// <summary>
        /// Updates volume level for the instrument.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        /// <param name="model">The model which describes level volume.</param>
        [Put("/api/instruments/{assetPairId}/levels")]
        Task UpdateLevelAsync(string assetPairId, [Body] LevelVolumeModel model);

        /// <summary>
        /// Removes the level volume from instrument.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        /// <param name="levelNumber">The number of the level.</param>
        [Delete("/api/instruments/{assetPairId}/levels/{levelNumber}")]
        Task RemoveLevelAsync(string assetPairId, int levelNumber);
    }
}
