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
        private const string CacheKey = "key";

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

                _cache.Initialize(new[] {state});
            }

            return state;
        }

        public Task SetStateAsync(MarketMakerStatus status, string comment, string userId)
            => SetStateAsync(status, MarketMakerError.None, null, comment, userId);

        public Task SetStateAsync(MarketMakerError marketMakerError, string errorMessages, string comment)
            => SetStateAsync(MarketMakerStatus.Error, marketMakerError, errorMessages, comment, null);

        private async Task SetStateAsync(MarketMakerStatus status, MarketMakerError marketMakerError,
            string errorMessages, string comment, string userId)
        {
            var marketMakerState = new MarketMakerState
            {
                Time = DateTime.UtcNow,
                Status = status,
                Error = marketMakerError,
                ErrorMessage = errorMessages
            };

            MarketMakerState currentMarketMakerState = await GetStateAsync();

            await _marketMakerStateRepository.InsertOrReplaceAsync(marketMakerState);

            _cache.Set(marketMakerState);

            _log.InfoWithDetails("Market maker state is changed.",
                new StateOperationContext(currentMarketMakerState.Status, marketMakerState.Status, comment, userId,
                    marketMakerState.Error, marketMakerState.ErrorMessage));
        }
    }
}
