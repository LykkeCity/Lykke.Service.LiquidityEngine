using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Publishers
{
    public interface IInternalQuotePublisher
    {
        Task PublishAsync(Quote quote);
    }
}
