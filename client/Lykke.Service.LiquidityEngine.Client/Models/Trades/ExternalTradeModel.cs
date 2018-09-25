using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.Trades
{
    /// <summary>
    /// Represents an external trade.
    /// </summary>
    [PublicAPI]
    public class ExternalTradeModel
    {
        /// <summary>
        /// The identifier of the trade.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The identifier of the limit order which executed while trade.
        /// </summary>
        public string LimitOrderId { get; set; }

        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The type of the trade.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public TradeType Type { get; set; }

        /// <summary>
        /// The time of the trade.
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// The price of the trade.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the trade.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The identifier of the request for quote.
        /// </summary>
        public string RequestId { get; set; }
        
        /// <summary>
        /// The price that was returned by external exchange for the request for quote.
        /// </summary>
        public decimal ProposedPrice { get; set; }
    }
}
