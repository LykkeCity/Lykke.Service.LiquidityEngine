using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with audit report.
    /// </summary>
    [PublicAPI]
    public interface IAuditApi
    {
        /// <summary>
        /// Returns a collection of balance operations for the period.
        /// </summary>
        /// <param name="startDate">The start date of period.</param>
        /// <param name="endDate">The end date of period.</param>
        /// <param name="limit">The maximum number of operations.</param>
        /// <returns>A collection of balance operations.</returns>
        [Get("/api/audit/balances")]
        Task<IReadOnlyCollection<BalanceOperationModel>> GetBalanceOperationsAsync(DateTime startDate, DateTime endDate, int limit);
    }
}
