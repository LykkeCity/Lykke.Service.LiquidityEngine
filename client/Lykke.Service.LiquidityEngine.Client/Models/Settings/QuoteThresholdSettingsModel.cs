using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settings
{
    /// <summary>
    /// Represents a settings of quote threshold.
    /// </summary>
    [PublicAPI]
    public class QuoteThresholdSettingsModel
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
