using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Reports
{
    /// <summary>
    /// Represents a summary report. 
    /// </summary>
    [PublicAPI]
    public class SummaryReportModel
    {
        /// <summary>
        /// The identifier of the asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The number of open positions.
        /// </summary>
        public int OpenPositionsCount { get; set; }

        /// <summary>
        /// The number of closed positions.
        /// </summary>
        public int ClosedPositionsCount { get; set; }

        /// <summary>
        /// The cumulative profit and loss.
        /// </summary>
        public decimal PnL { get; set; }
    }
}
