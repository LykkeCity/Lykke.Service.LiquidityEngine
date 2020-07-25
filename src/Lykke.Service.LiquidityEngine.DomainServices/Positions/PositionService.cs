using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Handlers;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.LiquidityEngine.DomainServices.Utils;

namespace Lykke.Service.LiquidityEngine.DomainServices.Positions
{
    public class PositionService : IPositionService
    {
        private readonly IPositionRepository _positionRepository;
        private readonly IPositionRepository _positionRepositoryPostgres;
        private readonly IOpenPositionRepository _openPositionRepository;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IInstrumentService _instrumentService;
        private readonly IQuoteService _quoteService;
        private readonly ITradeService _tradeService;
        private readonly IClosedPositionHandler[] _closedPositionHandlers;
        private readonly ILog _log;

        public PositionService(
            [KeyFilter("PositionRepositoryAzure")] IPositionRepository positionRepository,
            [KeyFilter("PositionRepositoryPostgres")] IPositionRepository positionRepositoryPostgres,
            IOpenPositionRepository openPositionRepository,
            ISummaryReportService summaryReportService,
            IInstrumentService instrumentService,
            IQuoteService quoteService,
            ITradeService tradeService,
            IClosedPositionHandler[] closedPositionHandlers,
            ILogFactory logFactory)
        {
            _positionRepository = positionRepository;
            _positionRepositoryPostgres = positionRepositoryPostgres;
            _openPositionRepository = openPositionRepository;
            _summaryReportService = summaryReportService;
            _instrumentService = instrumentService;
            _quoteService = quoteService;
            _tradeService = tradeService;
            _closedPositionHandlers = closedPositionHandlers;
            _log = logFactory.CreateLog(this);
        }

        public Task<IReadOnlyCollection<Position>> GetAllAsync(
            DateTime startDate, DateTime endDate, int limit, string assetPairId, string tradeAssetPairId)
        {
            return _positionRepository.GetAsync(startDate, endDate, limit, assetPairId, tradeAssetPairId);
        }

        public Task<IReadOnlyCollection<Position>> GetOpenAllAsync()
        {
            return _openPositionRepository.GetAllAsync();
        }

        public Task<IReadOnlyCollection<Position>> GetOpenByAssetPairIdAsync(string assetPairId)
        {
            return _openPositionRepository.GetByAssetPairIdAsync(assetPairId);
        }

        public async Task OpenAsync(IReadOnlyCollection<InternalTrade> internalTrades)
        {
            var positions = new List<Position>();

            foreach (InternalTrade internalTrade in internalTrades)
            {
                Instrument instrument = await _instrumentService.FindAsync(internalTrade.AssetPairId);

                if (instrument == null)
                {
                    _log.WarningWithDetails("Can not open position. Unknown instrument.", internalTrade);
                    continue;
                }

                Position position = null;

                if (internalTrade.AssetPairId != instrument.AssetPairId)
                {
                    CrossInstrument crossInstrument = instrument.CrossInstruments
                        .Single(o => o.AssetPairId == internalTrade.AssetPairId);

                    Quote quote = await _quoteService.GetAsync(crossInstrument.QuoteSource,
                        crossInstrument.ExternalAssetPairId);

                    if (quote != null)
                    {
                        decimal price = internalTrade.Type == TradeType.Sell
                            ? Calculator.CalculateDirectSellPrice(internalTrade.Price, quote, crossInstrument.IsInverse)
                            : Calculator.CalculateDirectBuyPrice(internalTrade.Price, quote, crossInstrument.IsInverse);

                        position = Position.Open(instrument.AssetPairId, price, internalTrade.Price,
                            internalTrade.Volume, quote, crossInstrument.AssetPairId, internalTrade.Type,
                            internalTrade.Id);

                        positions.Add(position);
                    }
                    else
                    {
                        _log.WarningWithDetails("Can not open position. No quote.", internalTrade);
                    }
                }
                else
                {
                    position = Position.Open(instrument.AssetPairId, internalTrade.Price, internalTrade.Volume,
                        internalTrade.Type, internalTrade.Id);

                    positions.Add(position);
                }

                var now = DateTime.UtcNow;

                if (position != null)
                {
                    _log.InfoWithDetails("Lykke trade handled", new
                    {
                        TradeId = internalTrade.Id,
                        PositionId = position.Id,
                        TradeTimestamp = internalTrade.Time,
                        Now = now,
                        Latency = (now - internalTrade.Time).TotalMilliseconds
                    });
                }
            }

            await _tradeService.RegisterAsync(internalTrades);

            foreach (Position position in positions)
            {
                await TraceWrapper.TraceExecutionTimeAsync("Inserting position to the Azure storage",
                    () => _positionRepository.InsertAsync(position), _log);

                await TraceWrapper.TraceExecutionTimeAsync("Inserting open position to the Azure storage",
                    () => _openPositionRepository.InsertAsync(position), _log);

                try
                {
                    await TraceWrapper.TraceExecutionTimeAsync("Inserting position to the Postgres storage",
                        () => _positionRepositoryPostgres.InsertAsync(position), _log);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An error occurred while inserting position to the Postgres DB",
                        position);
                }

                await TraceWrapper.TraceExecutionTimeAsync("Updating summary report in the Azure storage",
                    () => _summaryReportService.RegisterOpenPositionAsync(position,
                        internalTrades.Where(o => o.Id == position.TradeId).ToArray()), _log);

                _log.InfoWithDetails("Position opened", position);
            }
        }

        public async Task CloseAsync(InternalOrder internalOrder, ExternalTrade externalTrade)
        {
            await _tradeService.RegisterAsync(externalTrade);
            
            Position position = Position.Create(internalOrder, externalTrade);
            
            await _positionRepository.InsertAsync(position);

            try
            {
                await _positionRepositoryPostgres.InsertAsync(position);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception, "An error occurred while updating position in the postgres DB",
                    position);
            }

            await _summaryReportService.RegisterClosePositionAsync(position);

            _log.InfoWithDetails("Position closed", position);
        }
        
        public async Task CloseAsync(IReadOnlyCollection<Position> positions, ExternalTrade externalTrade)
        {
            await _tradeService.RegisterAsync(externalTrade);

            foreach (Position position in positions)
            {
                await _openPositionRepository.DeleteAsync(position.AssetPairId, position.Id);
            
                position.Close(externalTrade);

                await _positionRepository.UpdateAsync(position);

                try
                {
                    await _positionRepositoryPostgres.UpdateAsync(position);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An error occurred while updating position in the postgres DB",
                        position);
                }

                await _summaryReportService.RegisterClosePositionAsync(position);

                foreach (var closedPositionHandler in _closedPositionHandlers)
                    await closedPositionHandler.HandleClosedPositionAsync(position);

                _log.InfoWithDetails("Position closed", new
                {
                    position,
                    PositionId = position.Id,
                    Latency = (position.CloseDate - position.Date).TotalMilliseconds
                });
            }
        }

        public async Task CloseRemainingVolumeAsync(string assetPairId, ExternalTrade externalTrade)
        {
            await _tradeService.RegisterAsync(externalTrade);
            
            Position position = Position.Create(assetPairId, externalTrade);

            await _positionRepository.InsertAsync(position);

            try
            {
                await _positionRepositoryPostgres.InsertAsync(position);
            }
            catch (Exception exception)
            {
                _log.ErrorWithDetails(exception,
                    "An error occurred while inserting remaining volume position to the postgres DB", position);
            }

            _log.InfoWithDetails("Position with remaining volume closed", position);
        }
    }
}
