using System;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.OrderBooks;

namespace Lykke.Service.LiquidityEngine.Client.Models.InternalOrders
{
    /// <summary>
    /// Represents an order that used for internal trading.
    /// </summary>
    [PublicAPI]
    public class InternalOrderModel
    {
        /// <summary>
        /// The identifier of the order.
        /// </summary>
        public string Id { set; get; }

        /// <summary>
        /// The identifier of the wallet that should be used to transfer funds.
        /// </summary>
        public string WalletId { set; get; }

        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPairId { set; get; }

        /// <summary>
        /// The type of order.
        /// </summary>
        public LimitOrderType Type { set; get; }

        /// <summary>
        /// Desired price.
        /// </summary>
        public decimal Price { set; get; }

        /// <summary>
        /// Desired volume.
        /// </summary>
        public decimal Volume { set; get; }

        /// <summary>
        /// Actual price of execution.
        /// </summary>
        public decimal? ExecutedPrice { set; get; }

        /// <summary>
        /// Actual volume of execution.
        /// </summary>
        public decimal? ExecutedVolume { set; get; }

        /// <summary>
        /// If <c>true</c> the order should be fully executed; otherwise, part execution allowed.
        /// </summary>
        public bool FullExecution { set; get; }

        /// <summary>
        /// Status of the order.
        /// </summary>
        public InternalOrderStatus Status { set; get; }

        /// <summary>
        /// Rejection reason, in case order is rejected.
        /// </summary>
        public string RejectReason { set; get; }

        /// <summary>
        /// The identifier of trade.
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// The date and time of order creation.
        /// </summary>
        public DateTime CreatedDate { set; get; }
    }
}
