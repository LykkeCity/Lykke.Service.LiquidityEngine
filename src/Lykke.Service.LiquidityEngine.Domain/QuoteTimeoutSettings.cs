using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a settings of quote timeouts.
    /// </summary>
    public class QuoteTimeoutSettings
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
