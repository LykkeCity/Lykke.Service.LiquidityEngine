using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IQuoteTimeoutSettingsService
    {
        Task<QuoteTimeoutSettings> GetAsync();

        Task SaveAsync(QuoteTimeoutSettings quoteTimeoutSettings);
    }
}
