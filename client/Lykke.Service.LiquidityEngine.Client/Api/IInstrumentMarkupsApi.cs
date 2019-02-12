using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.InstrumentMarkups;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with instrument markups.
    /// </summary>
    [PublicAPI]
    public interface IInstrumentMarkupsApi
    {
        /// <summary>
        /// Returns a collection of instrument markups.
        /// </summary>
        /// <returns>Collection of instrument markups.</returns>
        [Get("/api/instrumentMarkups")]
        Task<IReadOnlyCollection<InstrumentMarkupModel>> GetAllAsync();
    }
}
