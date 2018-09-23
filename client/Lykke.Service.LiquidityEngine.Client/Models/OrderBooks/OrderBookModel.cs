using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents an order book.
    /// </summary>
    [PublicAPI]
    public class OrderBookModel
    {
        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The collection of limit orders from external exchange.
        /// </summary>
        public IReadOnlyCollection<LimitOrderModel> ExternalLimitOrders { get; set; }

        /// <summary>
        /// The collection of limit orders from internal exchange.
        /// </summary>
        public IReadOnlyCollection<LimitOrderModel> InternalLimitOrders { get; set; }
    }
}
