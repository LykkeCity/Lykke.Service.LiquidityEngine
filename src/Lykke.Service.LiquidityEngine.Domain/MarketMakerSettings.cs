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
        
        /// <summary>
        /// Common markup for limit orders.
        /// </summary>
        public decimal LimitOrderPriceMarkup { get; set; }

        /// <summary>
        /// Threshold from which MM should start applying <see cref="FiatEquityMarkupFrom"/>.
        /// </summary>
        public decimal FiatEquityThresholdFrom { get; set; }

        /// <summary>
        /// Threshold to stop providing 'ask' orders.
        /// </summary>
        public decimal FiatEquityThresholdTo { get; set; }

        /// <summary>
        /// Markup for <see cref="FiatEquityThresholdFrom"/>.
        /// </summary>
        public decimal FiatEquityMarkupFrom { get; set; }

        /// <summary>
        /// Markup for <see cref="FiatEquityThresholdTo"/>.
        /// </summary>
        public decimal FiatEquityMarkupTo { get; set; }
    }
}
