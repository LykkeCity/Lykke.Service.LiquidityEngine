using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Quotes
{
    /// <summary>
    /// Represents an asset pair quote.
    /// </summary>
    [PublicAPI]
    public class QuoteModel
    {
        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPair { get; set; }

        /// <summary>
        /// The creation time of quote.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The best price of sell limit order.
        /// </summary>
        public decimal Ask { get; set; }

        /// <summary>
        /// The best price of buy limit order.
        /// </summary>
        public decimal Bid { get; set; }

        /// <summary>
        /// The name of an exchange that provides a quote.
        /// </summary>
        public string Source { get; set; }
    }
}
