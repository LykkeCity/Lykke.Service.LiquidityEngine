using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.Positions
{
    /// <summary>
    /// Represents a position details.
    /// </summary>
    [PublicAPI]
    public class PositionModel
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
        public DateTime Date { get; set; }

        /// <summary>
        /// The price of the trade that opened the position.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// The volume of the trade that opened the position.
        /// </summary>
        public decimal Volume { get; set; }

        /// <summary>
        /// The date of the trade that closed the position.
        /// </summary>
        public DateTime CloseDate { get; set; }

        /// <summary>
        /// The price of the trade that closed the position.
        /// </summary>
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// The realised profit and loss.
        /// </summary>
        public decimal PnL { get; set; }

        /// <summary>
        /// A collection of identifiers of the trades that opened position.
        /// </summary>
        public IReadOnlyCollection<string> Trades { get; set; }

        /// <summary>
        /// The identifier of the trade that closed position.
        /// </summary>
        public string CloseTradeId { get; set; }
    }
}
