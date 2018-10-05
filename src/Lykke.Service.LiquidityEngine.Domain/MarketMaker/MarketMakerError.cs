namespace Lykke.Service.LiquidityEngine.Domain.MarketMaker
{
    /// <summary>
    /// Market maker error type
    /// </summary>
    public enum MarketMakerError
    {
        /// <summary>
        /// No error
        /// </summary>
        None = 0,

        /// <summary>
        /// An error during request happened
        /// </summary>
        IntegrationError = 1
    }
}
