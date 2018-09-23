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
    }
}
