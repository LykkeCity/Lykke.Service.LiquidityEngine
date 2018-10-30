namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represent a market maker settings.
    /// </summary>
    public class MarketMakerSettings
    {
        /// <summary>
        /// The maximum price deviation from first level. 
        /// </summary>
        public decimal LimitOrderPriceMaxDeviation { get; set; }
    }
}
