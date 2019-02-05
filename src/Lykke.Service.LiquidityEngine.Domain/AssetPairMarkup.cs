namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents asset pair markup.
    /// </summary>
    public class AssetPairMarkup
    {
        /// <summary>
        /// The identifier of asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Total markup.
        /// </summary>
        public decimal TotalMarkup { get; set; }
    }
}
