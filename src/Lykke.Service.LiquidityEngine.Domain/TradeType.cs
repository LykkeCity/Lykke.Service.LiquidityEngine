namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Specifies a type of a trade.
    /// </summary>
    public enum TradeType
    {
        /// <summary>
        /// Unspecified type.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that buy limit order was executed while trade.
        /// </summary>
        Buy,

        /// <summary>
        /// Indicates that sell limit order was executed while trade.
        /// </summary>
        Sell
    }
}
