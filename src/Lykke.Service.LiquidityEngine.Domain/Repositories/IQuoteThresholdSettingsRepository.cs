using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IQuoteThresholdSettingsRepository
    {
        Task<QuoteThresholdSettings> GetAsync();
        
        Task InsertOrReplaceAsync(QuoteThresholdSettings quoteThresholdSettings);
    }
}
