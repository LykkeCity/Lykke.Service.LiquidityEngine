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
        /// <param name="model">The model that describes instrument.</param>
        [Post("/api/instruments")]
        Task AddAsync([Body] InstrumentModel model);

        /// <summary>
        /// Updates instrument settings (without levels).
        /// </summary>
        /// <param name="model">The model that describes instrument.</param>
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
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="model">The model that describes level volume.</param>
        [Post("/api/instruments/{assetPairId}/levels")]
        Task AddLevelAsync(string assetPairId, [Body] InstrumentLevelModel model);

        /// <summary>
        /// Updates volume level for the instrument.
        /// </summary>
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="model">The model that describes level volume.</param>
        [Put("/api/instruments/{assetPairId}/levels")]
        Task UpdateLevelAsync(string assetPairId, [Body] InstrumentLevelModel model);

        /// <summary>
        /// Removes the level volume from instrument.
        /// </summary>
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="levelNumber">The number of the level.</param>
        [Delete("/api/instruments/{assetPairId}/levels/{levelNumber}")]
        Task RemoveLevelAsync(string assetPairId, int levelNumber);

        /// <summary>
        /// Adds the cross instrument to the main instrument.
        /// </summary>
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="model">The model that describes cross instrument.</param>
        [Post("/api/instruments/{assetPairId}/cross")]
        Task AddCrossInstrumentAsync(string assetPairId, [Body] CrossInstrumentModel model);

        /// <summary>
        /// Updates the cross instrument of the main instrument.
        /// </summary>
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="model">The model that describes cross instrument.</param>
        [Put("/api/instruments/{assetPairId}/cross")]
        Task UpdateCrossInstrumentAsync(string assetPairId, [Body] CrossInstrumentModel model);

        /// <summary>
        /// Removes the cross instrument from the main instrument.
        /// </summary>
        /// <param name="assetPairId">The identifier of instrument asset pair.</param>
        /// <param name="crossAssetPairId">The number of the level.</param>
        [Delete("/api/instruments/{assetPairId}/cross/{crossAssetPairId}")]
        Task RemoveCrossInstrumentAsync(string assetPairId, string crossAssetPairId);
    }
}
