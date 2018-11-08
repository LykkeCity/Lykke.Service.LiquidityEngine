using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Trades
{
    /// <summary>
    /// Represents time info about last internal trade.
    /// </summary>
    [PublicAPI]
    public class LastInternalTradeTimeModel
    {
        /// <summary>
        /// The identifier of asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Time of last internal trade.
        /// </summary>
        public DateTime LastInternalTradeTime { get; set; }
    }
}
