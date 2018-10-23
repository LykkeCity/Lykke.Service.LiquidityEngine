using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IQuoteService
    {
        Task<IReadOnlyCollection<Quote>> GetAsync();

        Task<Quote> GetAsync(string source, string assetPairId);

        Task SetAsync(Quote quote);
    }
}
