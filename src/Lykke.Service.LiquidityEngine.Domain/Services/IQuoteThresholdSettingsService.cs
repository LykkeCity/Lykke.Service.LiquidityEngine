using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IQuoteThresholdSettingsService
    {
        Task<QuoteThresholdSettings> GetAsync();

        Task UpdateAsync(QuoteThresholdSettings quoteThresholdSettings);
    }
}
