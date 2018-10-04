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
        /// The timer interval of lykke excahnge balances.
        /// </summary>
        public TimeSpan LykkeBalances { get; set; }

        /// <summary>
        /// The timer interval of external exchange balances.
        /// </summary>
        public TimeSpan ExternalBalances { get; set; }
    }
}
