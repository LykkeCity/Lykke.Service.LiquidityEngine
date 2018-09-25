using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lykke.Service.LiquidityEngine.Domain.Services
{
    public interface ITradeService
    {
        Task RegisterAsync(IReadOnlyCollection<InternalTrade> internalTrades);
        
        Task RegisterAsync(ExternalTrade externalTrade);
    }
}
