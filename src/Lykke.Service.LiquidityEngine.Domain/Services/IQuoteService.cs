using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IQuoteService
    {
        Task SetAsync(Quote quote);

        Task<Quote> GetAsync(string assetPairId);
    }
}
