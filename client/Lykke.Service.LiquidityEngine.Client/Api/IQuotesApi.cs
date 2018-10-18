using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Quotes;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with quotes.
    /// </summary>
    [PublicAPI]
    public interface IQuotesApi
    {
        /// <summary>
        /// Returns all existing quotes by all exchanges.
        /// </summary>
        /// <returns>A collection of quotes.</returns>
        [Get("/api/quotes")]
        Task<IReadOnlyCollection<QuoteModel>> GetAllAsync();
    }
}
