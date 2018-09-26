namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a limit order details.
    /// </summary>
    public class LimitOrder
    {
        /// <summary>
        /// The identifier of the limit order.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The limit order price.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The limit order volume.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The limit order type.
        /// </summary>
        public LimitOrderType Type { get; set; }
    }
}
