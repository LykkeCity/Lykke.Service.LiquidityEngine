using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.OrderBooks
{
    /// <summary>
    /// Specifies a limit order type.
    /// </summary>
    [PublicAPI]
    public enum LimitOrderType
    {
        /// <summary>
        /// Unspecified limit order type.
        /// </summary>
        None,

        /// <summary>
        /// Buy limit order type.
        /// </summary>
        Buy,

        /// <summary>
        /// Sell limit order type.
        /// </summary>
        Sell
    }
}
