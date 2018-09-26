using System;
using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a position details.
    /// </summary>
    public class Position
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
        public PositionType Type { get; set; }

        /// <summary>
        /// The date of the trade that opened the position
        /// </summary>
        public DateTime OpenDate { get; set; }

        /// <summary>
        /// The price of the trade that opened the position.
        /// </summary>
        public decimal OpenPrice { get; set; }

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
        public IReadOnlyCollection<string> OpenTrades { get; set; }

        /// <summary>
        /// The identifier of the trade that closed position.
        /// </summary>
        public string CloseTradeId { get; set; }
    }
}
