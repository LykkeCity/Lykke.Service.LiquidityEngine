using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Positions
{
    /// <summary>
    /// Specifies the type of a position. 
    /// </summary>
    [PublicAPI]
    public enum PositionType
    {
        /// <summary>
        /// Unspecified type.
        /// </summary>
        None,

        /// <summary>
        /// A short position that was opened by selling an asset with the expectation that the asset will decrease in value.
        /// </summary>
        Short,

        /// <summary>
        /// A long position that was opened by buying an asset with the expectation that the asset will rise in value.
        /// </summary>
        Long
    }
}
