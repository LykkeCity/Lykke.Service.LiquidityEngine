using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with cross instruments used to calculate prices.
    /// </summary>
    [PublicAPI]
    public interface ICrossRateInstrumentsApi
    {
        /// <summary>
        /// Returns a collection of cross instruments.
        /// </summary>
        /// <returns>A collection of cross instruments.</returns>
        [Get("/api/crossrateinstruments")]
        Task<IReadOnlyCollection<CrossRateInstrumentModel>> GetAllAsync();

        /// <summary>
        /// Returns a cross instrument by asset pair id.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        /// <returns>A cross instrument.</returns>
        [Get("/api/crossrateinstruments/{assetPairId}")]
        Task<CrossRateInstrumentModel> GetByAssetPairIdAsync(string assetPairId);

        /// <summary>
        /// Adds new cross instrument settings.
        /// </summary>
        /// <param name="model">The model which describes cross instrument.</param>
        [Post("/api/crossrateinstruments")]
        Task AddAsync([Body] CrossRateInstrumentModel model);

        /// <summary>
        /// Updates cross instrument settings.
        /// </summary>
        /// <param name="model">The model which describes cross instrument.</param>
        [Put("/api/crossrateinstruments")]
        Task UpdateAsync([Body] CrossRateInstrumentModel model);

        /// <summary>
        /// Deletes the cross instrument settings by asset pair id.
        /// </summary>
        /// <param name="assetPairId">The asses pair id.</param>
        [Delete("/api/crossrateinstruments/{assetPairId}")]
        Task DeleteAsync(string assetPairId);
    }
}
