using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.AttributeFilters;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

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
        private readonly ILog _log;

        public PositionService(
            [KeyFilter("PositionRepositoryAzure")] IPositionRepository positionRepository,
            [KeyFilter("PositionRepositoryPostgres")] IPositionRepository positionRepositoryPostgres,
            IOpenPositionRepository openPositionRepository,
            ISummaryReportService summaryReportService,
            IInstrumentService instrumentService,
            IQuoteService quoteService,
            ILogFactory logFactory)
        {
            _positionRepository = positionRepository;
            _positionRepositoryPostgres = positionRepositoryPostgres;
            _openPositionRepository = openPositionRepository;
            _summaryReportService = summaryReportService;
            _instrumentService = instrumentService;
            _quoteService = quoteService;
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
            if (internalTrades.Count == 0)
                return;

            string assetPairId = internalTrades.First().AssetPairId;

            TradeType tradeType = internalTrades.First().Type;

            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            Instrument instrument = instruments.FirstOrDefault(o =>
                o.AssetPairId == assetPairId || o.CrossInstruments.Any(p => p.AssetPairId == assetPairId));

            if (instrument == null)
            {
                _log.WarningWithDetails($"Can not open position. Unknown instrument '{assetPairId}'.", internalTrades);
                return;
            }

            var positions = new List<Position>();

            if (assetPairId != instrument.AssetPairId)
            {
                CrossInstrument crossInstrument = instrument.CrossInstruments.Single(o => o.AssetPairId == assetPairId);

                Quote quote =
                    await _quoteService.GetAsync(crossInstrument.QuoteSource, crossInstrument.ExternalAssetPairId);

                if (quote == null)
                {
                    _log.WarningWithDetails($"Can not open position. No quote '{assetPairId}'.", internalTrades);
                    return;
                }

                foreach (InternalTrade internalTrade in internalTrades)
                {
                    decimal price = tradeType == TradeType.Sell
                        ? Calculator.CalculateDirectSellPrice(internalTrade.Price, quote, crossInstrument.IsInverse)
                        : Calculator.CalculateDirectBuyPrice(internalTrade.Price, quote, crossInstrument.IsInverse);

                    positions.Add(Position.Open(instrument.AssetPairId, price, internalTrade.Price,
                        internalTrade.Volume, quote, crossInstrument.AssetPairId, tradeType, internalTrade.Id));
                }
            }
            else
            {
                foreach (InternalTrade internalTrade in internalTrades)
                {
                    positions.Add(Position.Open(instrument.AssetPairId, internalTrade.Price, internalTrade.Volume,
                        tradeType, internalTrade.Id));
                }
            }
            
            var sw = new Stopwatch();
            
            foreach (Position position in positions)
            {
                sw.Reset();
                sw.Start();

                _log.Info("Inserting position to the Azure storage");
                
                try
                {
                    await _openPositionRepository.InsertAsync(position);

                    await _positionRepository.InsertAsync(position);
                }
                finally
                {
                    sw.Stop();
                    _log.Info("Position inserted to the Azure storage", new {sw.ElapsedMilliseconds});
                }

                sw.Reset();
                sw.Start();
                
                _log.Info("Inserting position to the Postgres storage");
                
                try
                {
                    await _positionRepositoryPostgres.InsertAsync(position);
                }
                catch (Exception exception)
                {
                    _log.ErrorWithDetails(exception, "An error occurred while inserting position to the Postgres DB",
                        position);
                }
                finally
                {
                    sw.Stop();
                    _log.Info("Position inserted to the Postgres storage", new {sw.ElapsedMilliseconds});
                }
                
                sw.Reset();
                sw.Start();
                
                _log.Info("Updating summary report in the Azure storage");
                
                try
                {
                    await _summaryReportService.RegisterOpenPositionAsync(position,
                        internalTrades.Where(o => o.Id == position.TradeId).ToArray());
                }
                finally
                {
                    sw.Stop();
                    _log.Info("Summary report updated int the Azure storage", new {sw.ElapsedMilliseconds});
                }

                _log.InfoWithDetails("Position was opened", position);
            }
        }

        public async Task CloseAsync(Position position, ExternalTrade externalTrade)
        {
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

            await _openPositionRepository.DeleteAsync(position.AssetPairId, position.Id);

            await _summaryReportService.RegisterClosePositionAsync(position);

            _log.InfoWithDetails("Position was closed", position);
        }

        public async Task CloseRemainingVolumeAsync(string assetPairId, ExternalTrade externalTrade)
        {
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

            _log.InfoWithDetails("Position with remaining volume was closed", position);
        }
    }
}
