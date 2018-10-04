using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.OrderBooks
{
    /// <summary>
    /// Represents a limit order details.
    /// </summary>
    [PublicAPI]
    public class LimitOrderModel
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
        [JsonConverter(typeof(StringEnumConverter))]
        public LimitOrderType Type { get; set; }
        
        /// <summary>
        /// The code of the error which occurred while processing on ME.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public LimitOrderError Error { get; set; }

        /// <summary>
        /// The error details.
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
