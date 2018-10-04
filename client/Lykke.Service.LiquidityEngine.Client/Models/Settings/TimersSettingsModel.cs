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
        /// The timer interval of balances.
        /// </summary>
        public TimeSpan Balances { get; set; }
    }
}
