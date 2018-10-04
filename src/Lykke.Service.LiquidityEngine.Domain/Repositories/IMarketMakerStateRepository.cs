using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;

namespace Lykke.Service.LiquidityEngine.Domain.Repositories
{
    public interface IMarketMakerStateRepository
    {
        Task<MarketMakerState> GetAsync();

        Task InsertOrReplaceAsync(MarketMakerState state);
    }
}
