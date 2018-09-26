using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Trades
{
    [UsedImplicitly]
    public class TradeService : ITradeService
    {
        private readonly IInternalTradeRepository _internalTradeRepository;
        private readonly IExternalTradeRepository _externalTradeRepository;

        public TradeService(
            IInternalTradeRepository internalTradeRepository,
            IExternalTradeRepository externalTradeRepository)
        {
            _internalTradeRepository = internalTradeRepository;
            _externalTradeRepository = externalTradeRepository;
        }

        public Task<IReadOnlyCollection<ExternalTrade>> GetExternalTradesAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            return _externalTradeRepository.GetAsync(startDate, endDate, limit);
        }

        public Task<ExternalTrade> GetExternalTradeByIdAsync(string externalTradeId)
        {
            return _externalTradeRepository.GetByIdAsync(externalTradeId);
        }

        public Task<IReadOnlyCollection<InternalTrade>> GetInternalTradesAsync(DateTime startDate, DateTime endDate,
            int limit)
        {
            return _internalTradeRepository.GetAsync(startDate, endDate, limit);
        }

        public Task<InternalTrade> GetInternalTradeByIdAsync(string internalTradeId)
        {
            return _internalTradeRepository.GetByIdAsync(internalTradeId);
        }

        public Task RegisterAsync(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            return Task.WhenAll(internalTrades.Select(internalTrade =>
                _internalTradeRepository.InsertAsync(internalTrade)));
        }

        public Task RegisterAsync(ExternalTrade externalTrade)
        {
            return _externalTradeRepository.InsertAsync(externalTrade);
        }
    }
}
