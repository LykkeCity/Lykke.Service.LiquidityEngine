using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Domain.Cache
{
    public interface IBalanceCache
    {
        IReadOnlyCollection<Balance> Get();

        IReadOnlyCollection<Balance> Get(string exchange);

        Balance Get(string exchange, string assetId);

        void Set(IReadOnlyCollection<Balance> balance);
    }
}
