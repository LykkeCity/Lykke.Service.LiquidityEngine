using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.InternalOrders;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with internal orders.
    /// </summary>
    [PublicAPI]
    public interface IInternalOrdersApi
    {
        /// <summary>
        /// Returns a collection of internal orders.
        /// </summary>
        /// <returns>A collection of internal orders.</returns>
        [Get("/api/InternalOrders")]
        Task<IReadOnlyCollection<InternalOrderModel>> GetAsync(DateTime startDate, DateTime endDate, int limit);
    }
}
