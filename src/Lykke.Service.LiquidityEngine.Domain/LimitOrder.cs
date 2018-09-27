using System;

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

        /// <summary>
        /// The code of the error which occurred while processing on ME.
        /// </summary>
        public LimitOrderError Error { get; set; }

        /// <summary>
        /// The error details.
        /// </summary>
        public string ErrorMessage { get; set; }

        public static LimitOrder CreateSell(decimal price, decimal volume)
            => Create(price, volume, LimitOrderType.Sell);

        public static LimitOrder CreateBuy(decimal price, decimal volume)
            => Create(price, volume, LimitOrderType.Buy);

        private static LimitOrder Create(decimal price, decimal volume, LimitOrderType type)
            => new LimitOrder
            {
                Id = Guid.NewGuid().ToString("D"),
                Price = price,
                Volume = volume,
                Type = type
            };
    }
}
