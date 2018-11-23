using System;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.Reports
{
    /// <summary>
    /// Represent position details.
    /// </summary>
    [PublicAPI]
    public class PositionReportModel
    {
        /// <summary>
        /// The identifier of the position.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The type of position.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public PositionType Type { get; set; }

        /// <summary>
        /// The date when position was opened. 
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The price of the trade that opened the position.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the trade that opened the position.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// Indicates that the position closed.
        /// </summary>
        public bool IsClosed { get; set; }

        /// <summary>
        /// The date of the trade that closed the position.
        /// </summary>
        public DateTime? CloseDate { get; set; }

        /// <summary>
        /// The price of the trade that closed the position.
        /// </summary>
        public decimal? ClosePrice { get; set; }

        /// <summary>
        /// The realised profit and loss.
        /// </summary>
        public decimal? PnL { get; set; }

        /// <summary>
        /// The realised profit and loss.
        /// </summary>
        public decimal? PnLUsd { get; set; }

        /// <summary>
        /// The best sell price on trade time.
        /// </summary>
        public decimal? CrossAsk { get; set; }

        /// <summary>
        /// The best buy price on trade time.
        /// </summary>
        public decimal? CrossBid { get; set; }

        /// <summary>
        /// The identifier of asset pair that used to convert trade price.
        /// </summary>
        public string CrossAssetPairId { get; set; }

        /// <summary>
        /// The identifier of asset pair that used by trade.
        /// </summary>
        public string TradeAssetPairId { get; set; }

        /// <summary>
        /// The identifier of the internal trade that opened position.
        /// </summary>
        public string InternalTradesId { get; set; }

        /// <summary>
        /// The identifier of the external edge trade that closed position.
        /// </summary>
        public string ExternalTradeId { get; set; }
    }
}
