using System;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.MarketMaker;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.MarketMaker
{
    public class MarketMakerStateService : IMarketMakerStateService
    {
        private const string CacheKey = "MarketMakerState";

        private readonly IMarketMakerStateRepository _marketMakerStateRepository;
        private readonly InMemoryCache<MarketMakerState> _cache;
        private readonly ILog _log;

        public MarketMakerStateService(IMarketMakerStateRepository marketMakerStateRepository, ILogFactory logFactory)
        {
            _marketMakerStateRepository = marketMakerStateRepository;
            _cache = new InMemoryCache<MarketMakerState>(settings => CacheKey, true);
            _log = logFactory.CreateLog(this);
        }

        public async Task<MarketMakerState> GetStateAsync()
        {
            MarketMakerState state = _cache.Get(CacheKey);

            if (state == null)
            {
                state = await _marketMakerStateRepository.GetAsync();

                if (state == null)
                {
                    state = new MarketMakerState
                    {
                        Status = MarketMakerStatus.Disabled,
                        Time = DateTime.UtcNow
                    };
                }

                _cache.Initialize(new[] { state });
            }

            return state;
        }

        public async Task SetStateAsync(MarketMakerState state, string comment, string userId = null)
        {
            MarketMakerState currentState = await GetStateAsync();

            await _marketMakerStateRepository.InsertOrReplaceAsync(state);

            _cache.Set(state);

            _log.InfoWithDetails("Market maker state is changed.", 
                new StateOperationContext(currentState.Status, state.Status, comment, userId, state.Error, state.ErrorMessage));
        }
    }
}
