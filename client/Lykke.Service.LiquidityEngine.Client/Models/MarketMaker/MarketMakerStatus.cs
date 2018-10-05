namespace Lykke.Service.LiquidityEngine.Client.Models.MarketMaker
{
    /// <summary>
    /// Market maker status
    /// </summary>
    public enum MarketMakerStatus
    {
        /// <summary>
        /// Unspecified type
        /// </summary>
        None = 0,

        /// <summary>
        /// Market maker is active
        /// </summary>
        Active = 1,

        /// <summary>
        /// Market maker is disabled
        /// </summary>
        Disabled = 2,

        /// <summary>
        /// Market maker has error
        /// </summary>
        Error = 3
    }
}
