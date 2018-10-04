using System;
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
        /// The date and time of creation.
        /// </summary>
        public DateTime Time { get; set; }
        
        /// <summary>
        /// The collection of limit orders.
        /// </summary>
        public IReadOnlyCollection<LimitOrderModel> LimitOrders { get; set; }
    }
}
