using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with remaining volume.
    /// </summary>
    [PublicAPI]
    public interface IRemainingVolumeApi
    {
        /// <summary>
        /// Returns a collection of remaining volumes.
        /// </summary>
        /// <returns>A collection of remaining volumes.</returns>
        [Get("/api/remainingvolumes")]
        Task<IReadOnlyCollection<RemainingVolumeModel>> GetAllAsync();
    }
}
