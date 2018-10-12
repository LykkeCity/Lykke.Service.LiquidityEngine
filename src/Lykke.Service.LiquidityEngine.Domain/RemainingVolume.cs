namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a instrument cumulative remaining volume. 
    /// </summary>
    public class RemainingVolume
    {
        /// <summary>
        /// The identifier of an internal asset pair. 
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The cumulative remaining volume.
        /// </summary>
        public decimal Volume { get; set; }

        public void Add(decimal volume)
        {
            Volume += volume;
        }

        public void Subtract(decimal volume)
        {
            Volume += volume;
        }
    }
}
