using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settings
{
    /// <summary>
    /// Represents a settings of timers.
    /// </summary>
    [PublicAPI]
    public class TimersSettingsModel
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
        /// The timer interval of Lykke exchange balances.
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
