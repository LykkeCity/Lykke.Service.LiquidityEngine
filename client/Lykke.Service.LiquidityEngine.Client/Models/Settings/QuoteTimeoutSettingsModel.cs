using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settings
{
    /// <summary>
    /// Represents a settings of quote timeouts.
    /// </summary>
    [PublicAPI]
    public class QuoteTimeoutSettingsModel
    {
        /// <summary>
        /// Indicates that the timeout is enabled.
        /// </summary>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// The timeout of the quote then error will be raced.
        /// </summary>
        public TimeSpan Error { get; set; }
    }
}
