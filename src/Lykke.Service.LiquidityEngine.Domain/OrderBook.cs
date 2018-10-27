using System;
using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The identifier of asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The date and time of creation.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Indicated that the order book created by direct instrument.
        /// </summary>
        public bool IsDirect { get; set; }

        /// <summary>
        /// The identifier of asset pair that used as base order book.
        /// </summary>
        public string BaseAssetPairId { get; set; }

        /// <summary>
        /// The cross quote that used to calculate order book.
        /// </summary>
        public Quote CrossQuote { get; set; }

        /// <summary>
        /// The collection of limit orders.
        /// </summary>
        public IReadOnlyCollection<LimitOrder> LimitOrders { get; set; }
    }
}
