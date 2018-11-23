using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Reports;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with reports.
    /// </summary>
    [PublicAPI]
    public interface IReportsApi
    {
        /// <summary>
        /// Returns summary report for each asset pair.
        /// </summary>
        /// <returns>A collection of asset pair summary info.</returns>
        [Get("/api/reports/summary")]
        Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportAsync();

        /// <summary>
        /// Returns summary report for each asset pair for specified period.
        /// </summary>
        /// <returns>A collection of asset pair summary info.</returns>
        [Get("/api/reports/summaryByPeriod")]
        Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportByPeriodAsync(
            DateTime startDate, DateTime endDate);

        /// <summary>
        /// Returns position report.
        /// </summary>
        /// <returns>A collection of positions reports.</returns>
        [Get("/api/reports/positions")]
        Task<IReadOnlyCollection<PositionReportModel>> GetPositionsReportAsync(DateTime startDate, DateTime endDate,
            int limit);

        /// <summary>
        /// Returns balance report for each asset.
        /// </summary>
        /// <returns>A collection of asset balance info.</returns>
        [Get("/api/reports/balances")]
        Task<IReadOnlyCollection<BalanceReportModel>> GetBalancesReportAsync();
    }
}
