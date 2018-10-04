using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents settings of service timers. 
    /// </summary>
    public class TimersSettings
    {
        /// <summary>
        /// The timer interval of market maker.
        /// </summary>
        public TimeSpan MarketMaker { get; set; }
        
        /// <summary>
        /// The timer interval of balances.
        /// </summary>
        public TimeSpan Balances { get; set; }
    }
}
