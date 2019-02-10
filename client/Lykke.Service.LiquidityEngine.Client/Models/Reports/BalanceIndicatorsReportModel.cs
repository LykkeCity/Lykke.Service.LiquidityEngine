using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Reports
{
    /// <summary>
    /// Represents a report on current balance indicator.
    /// </summary>
    [PublicAPI]
    public class BalanceIndicatorsReportModel
    {
        /// <summary>
        /// The equity indicator.
        /// </summary>
        public decimal Equity { get; set; }

        /// <summary>
        /// The fiat equity indicator.
        /// </summary>
        public decimal FiatEquity { get; set; }

        /// <summary>
        /// The risk exposure indicator.
        /// </summary>
        public decimal RiskExposure { get; set; }
    }
}
