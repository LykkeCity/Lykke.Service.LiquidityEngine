using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settings
{
    /// <summary>
    /// Represent a market maker settings.
    /// </summary>
    [PublicAPI]
    public class MarketMakerSettingsModel
    {
        /// <summary>
        /// The maximum price deviation from first level. 
        /// </summary>
        public decimal LimitOrderPriceMaxDeviation { get; set; }
        
        /// <summary>
        /// Common markup for limit orders.
        /// </summary>
        public decimal LimitOrderPriceMarkup { get; set; }
    }
}
