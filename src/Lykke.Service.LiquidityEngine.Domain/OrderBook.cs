using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    public class OrderBook
    {
        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The collection of limit orders from external exchange.
        /// </summary>
        public IReadOnlyCollection<LimitOrder> ExternalLimitOrders { get; set; }

        /// <summary>
        /// The collection of limit orders from internal exchange.
        /// </summary>
        public IReadOnlyCollection<LimitOrder> InternalLimitOrders { get; set; }
    }
}
