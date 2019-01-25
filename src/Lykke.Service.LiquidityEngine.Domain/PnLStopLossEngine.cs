using System;

namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a pnl stop loss engine.
    /// </summary>
    public class PnLStopLossEngine
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
        /// PnL stop loss settings identifier (for global setting only).
        /// </summary>
        public string PnLStopLossGlobalSettingsId { get; set; }

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

        /// <summary>
        /// Total negative PnL.
        /// </summary>
        public decimal TotalNegativePnL { get; set; }

        /// <summary>
        /// First time when negative PnL occured.
        /// </summary>
        public DateTime FirstTime { get; set; }

        /// <summary>
        /// Last time when negative PnL occured.
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// The mode of the pnl stop loss engine.
        /// </summary>
        public PnLStopLossEngineMode Mode { get; set; }

        public static PnLStopLossEngine CreateFromLocalSettings(PnLStopLossSettings pnLStopLossSettings)
        {
            return new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                AssetPairId = pnLStopLossSettings.AssetPairId,
                Interval = pnLStopLossSettings.Interval,
                PnLThreshold = pnLStopLossSettings.PnLThreshold,
                Markup = pnLStopLossSettings.Markup,
                PnLStopLossGlobalSettingsId = null,
                TotalNegativePnL = 0,
                FirstTime = default(DateTime),
                LastTime = default(DateTime),
                Mode = PnLStopLossEngineMode.Idle
            };
        }

        public static PnLStopLossEngine CreateFromGlobalSettings(string assetPairId, PnLStopLossSettings pnLStopLossSettings)
        {
            return new PnLStopLossEngine
            {
                Id = Guid.NewGuid().ToString(),
                AssetPairId = assetPairId,
                Interval = pnLStopLossSettings.Interval,
                PnLThreshold = pnLStopLossSettings.PnLThreshold,
                Markup = pnLStopLossSettings.Markup,
                PnLStopLossGlobalSettingsId = pnLStopLossSettings.Id,
                TotalNegativePnL = 0,
                FirstTime = default(DateTime),
                LastTime = default(DateTime),
                Mode = PnLStopLossEngineMode.Idle
            };
        }

        public void ApplyNewPosition(Position position)
        {
            if (TotalNegativePnL == 0)
                FirstTime = DateTime.UtcNow;

            TotalNegativePnL += position.PnL;

            LastTime = DateTime.UtcNow;
        }

        public void Refresh()
        {
            if (TotalNegativePnL == 0)
                return;

            if (DateTime.UtcNow - FirstTime < Interval)
                return;

            if (TotalNegativePnL > PnLThreshold)
            {
                Reset();
                return;
            }

            if (DateTime.UtcNow - LastTime > Interval)
                Reset();
        }

        public void ChangeMode(PnLStopLossEngineMode mode)
        {
            Mode = mode;
        }

        public void Reset()
        {
            TotalNegativePnL = 0;
            FirstTime = default(DateTime);
            LastTime = default(DateTime);
        }
    }
}
