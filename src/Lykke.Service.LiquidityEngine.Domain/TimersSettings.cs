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
        /// The timer interval of hedge algorithm.
        /// </summary>
        public TimeSpan Hedging { get; set; }

        /// <summary>
        /// The timer interval of lykke exchange balances.
        /// </summary>
        public TimeSpan LykkeBalances { get; set; }

        /// <summary>
        /// The timer interval of external exchange balances.
        /// </summary>
        public TimeSpan ExternalBalances { get; set; }
        
        /// <summary>
        /// The timer interval of B2C2 settlements operations.
        /// </summary>
        public TimeSpan Settlements { get; set; }
    }
}
