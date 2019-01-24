using System;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.PnLStopLosses
{
    /// <summary>
    /// Represents a pnl stop loss settings.
    /// </summary>
    [PublicAPI]
    public class PnLStopLossSettingsModel
    {
        /// <summary>
        /// Identifier.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Asset pair identifier.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Time interval for calculating loss.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// PnL threshold.
        /// </summary>
        public decimal PnLThreshold { get; set; }

        /// <summary>
        /// Markup.
        /// </summary>
        public decimal Markup { get; set; }
    }
}
