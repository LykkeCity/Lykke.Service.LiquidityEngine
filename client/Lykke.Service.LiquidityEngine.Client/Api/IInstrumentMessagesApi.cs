using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.InstrumentMessages;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with instrument messages.
    /// </summary>
    [PublicAPI]
    public interface IInstrumentMessagesApi
    {
        /// <summary>
        /// Returns a collection of instrument messages.
        /// </summary>
        /// <returns>Collection of instrument messages.</returns>
        [Get("/api/instrumentMessages")]
        Task<IReadOnlyCollection<InstrumentMessagesModel>> GetAllAsync();
    }
}
