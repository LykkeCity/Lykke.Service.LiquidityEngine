namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a current balance indicators report.
    /// </summary>
    public class BalanceIndicatorsReport
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
