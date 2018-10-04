namespace Lykke.Service.LiquidityEngine.Client.Models.MarketMaker
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
        /// Max risk exposure reached
        /// </summary>
        MaxRiskExposure = 1,

        /// <summary>
        /// Max credit exposure reached
        /// </summary>
        MaxCreditExposure = 2
    }
}
