using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Positions
{
    /// <summary>
    /// Represents a instrument cumulative remaining volume. 
    /// </summary>
    [PublicAPI]
    public class RemainingVolumeModel
    {
        /// <summary>
        /// The identifier of an internal asset pair. 
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The cumulative remaining volume.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
