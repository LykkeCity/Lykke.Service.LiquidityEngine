namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface IQuoteThresholdLogService
    {
        void Error(Quote lastQuote, Quote currentQuote, decimal threshold);
    }
}
