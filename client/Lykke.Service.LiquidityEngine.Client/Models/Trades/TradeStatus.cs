using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Trades
{
    /// <summary>
    /// Specifies a status of a trade.
    /// </summary>
    [PublicAPI]
    public enum TradeStatus
    {
        /// <summary>
        /// Unspecified status.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that trade was fully executed. 
        /// </summary>
        Fill,

        /// <summary>
        /// Indicates that trade was partially executed. 
        /// </summary>
        PartialFill
    }
}
