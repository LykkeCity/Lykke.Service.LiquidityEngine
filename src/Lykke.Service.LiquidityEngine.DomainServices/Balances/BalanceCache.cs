using System.Collections.Generic;
using System.Linq;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Cache;

namespace Lykke.Service.LiquidityEngine.DomainServices.Balances
{
    public class BalanceCache : IBalanceCache
    {
        private readonly object _sync = new object();

        private readonly Dictionary<string, Dictionary<string, Balance>> _balances =
            new Dictionary<string, Dictionary<string, Balance>>();

        public IReadOnlyCollection<Balance> Get()
        {
            lock (_sync)
            {
                return _balances.Values
                    .SelectMany(o => o.Values)
                    .ToList();
            }
        }

        public IReadOnlyCollection<Balance> Get(string exchange)
        {
            lock (_sync)
            {
                if (_balances.ContainsKey(exchange))
                    return _balances[exchange].Values.ToList();

                return new List<Balance>();
            }
        }

        public Balance Get(string exchange, string assetId)
        {
            lock (_sync)
            {
                if (_balances.ContainsKey(exchange) && _balances[exchange].ContainsKey(assetId))
                    return _balances[exchange][assetId];
            }

            return null;
        }

        public void Set(IReadOnlyCollection<Balance> balances)
        {
            lock (_sync)
            {
                foreach (IGrouping<string, Balance> group in balances.GroupBy(o => o.Exchange))
                    _balances[group.Key] = group.ToDictionary(o => o.AssetId);
            }
        }
    }
}
