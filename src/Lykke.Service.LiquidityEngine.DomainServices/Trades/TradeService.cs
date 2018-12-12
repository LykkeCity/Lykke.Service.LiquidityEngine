using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;

namespace Lykke.Service.LiquidityEngine.DomainServices.Trades
{
    [UsedImplicitly]
    public class TradeService : ITradeService
    {
        private readonly IInternalTradeRepository _internalTradeRepository;
        private readonly IInternalTradeRepository _internalTradeRepositoryPostgres;
        private readonly IExternalTradeRepository _externalTradeRepository;
        private readonly IExternalTradeRepository _externalTradeRepositoryPostgres;
        private readonly ILog _log;

        private readonly ConcurrentDictionary<string, DateTime> _internalLastInternalTradeTime =
            new ConcurrentDictionary<string, DateTime>();

        private bool _initialized;
        private DateTime _defaultTradeTime;

        public TradeService(
            [KeyFilter("InternalTradeRepositoryAzure")]
            IInternalTradeRepository internalTradeRepository,
            [KeyFilter("InternalTradeRepositoryPostgres")]
            IInternalTradeRepository internalTradeRepositoryPostgres,
            [KeyFilter("ExternalTradeRepositoryAzure")]
            IExternalTradeRepository externalTradeRepository,
            [KeyFilter("ExternalTradeRepositoryPostgres")]
            IExternalTradeRepository externalTradeRepositoryPostgres,
            ILogFactory logFactory)
        {
            _internalTradeRepository = internalTradeRepository;
            _internalTradeRepositoryPostgres = internalTradeRepositoryPostgres;
            _externalTradeRepository = externalTradeRepository;
            _externalTradeRepositoryPostgres = externalTradeRepositoryPostgres;
            _log = logFactory.CreateLog(this);
        }

        public void Initialize()
        {
            if (!_initialized)
            {
                _initialized = true;
                _defaultTradeTime = DateTime.UtcNow;
            }
        }

        public DateTime GetLastInternalTradeTime(string assetPairId)
        {
            if (!_internalLastInternalTradeTime.TryGetValue(assetPairId, out DateTime time))
                time = _defaultTradeTime;

            return time;
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

        public async Task RegisterAsync(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            foreach (IGrouping<string, InternalTrade> group in internalTrades.GroupBy(o => o.AssetPairId))
            {
                DateTime lastTradeTime = group.Max(o => o.Time);

                _internalLastInternalTradeTime.AddOrUpdate(group.Key, lastTradeTime,
                    (assetPairId, time) => lastTradeTime);
            }

            foreach (InternalTrade internalTrade in internalTrades)
            {
                await TraceWrapper.TraceExecutionTimeAsync("Inserting internal trade to the Azure storage",
                    () => _internalTradeRepository.InsertAsync(internalTrade), _log);

                try
                {
                    await TraceWrapper.TraceExecutionTimeAsync("Inserting internal trade to the Postgres storage",
                        () => _internalTradeRepositoryPostgres.InsertAsync(internalTrade), _log);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception,
                        "An error occurred while inserting internal trade to the Postgres DB", internalTrade);
                }
            }
        }

        public async Task RegisterAsync(ExternalTrade externalTrade)
        {
            await TraceWrapper.TraceExecutionTimeAsync("Inserting external trade to the Azure storage",
                () => _externalTradeRepository.InsertAsync(externalTrade), _log);

            try
            {
                await TraceWrapper.TraceExecutionTimeAsync("Inserting external trade to the Postgres storage",
                    () => _externalTradeRepositoryPostgres.InsertAsync(externalTrade), _log);

                await _externalTradeRepositoryPostgres.InsertAsync(externalTrade);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception,
                    "An error occurred while inserting external trade to the Postgres DB", externalTrade);
            }
        }
    }
}
