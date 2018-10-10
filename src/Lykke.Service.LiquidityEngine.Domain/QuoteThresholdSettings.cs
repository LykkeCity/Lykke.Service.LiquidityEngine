namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a settings of quote threshold.
    /// </summary>
    public class QuoteThresholdSettings
    {
        /// <summary>
        /// Indicates that the threshold is enabled.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The value of threshold.
        /// </summary>
        public decimal Value { get; set; }
    }
}
