using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a pnl stop loss settings.
    /// </summary>
    public class PnLStopLossSettings
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Time interval for calculating loss.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// PnL threshold.
        /// </summary>
        public decimal Threshold { get; set; }

        /// <summary>
        /// Markup.
        /// </summary>
        public decimal Markup { get; set; }

        public PnLStopLossSettings()
        {
        }

        public PnLStopLossSettings(PnLStopLossSettings pnLStopLossSettings)
        {
            Id = Guid.NewGuid().ToString();
            Interval = pnLStopLossSettings.Interval;
            Threshold = pnLStopLossSettings.Threshold;
            Markup = pnLStopLossSettings.Markup;
        }
    }
}
