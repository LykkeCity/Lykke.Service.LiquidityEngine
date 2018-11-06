using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly IOpenPositionRepository _openPositionRepository;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IInstrumentService _instrumentService;
        private readonly IQuoteService _quoteService;
        private readonly IRateService _rateService;
        private readonly ILog _log;

        public PositionService(
            IPositionRepository positionRepository,
            IOpenPositionRepository openPositionRepository,
            ISummaryReportService summaryReportService,
            IInstrumentService instrumentService,
            IQuoteService quoteService,
            IRateService rateService,
            ILogFactory logFactory)
        {
            _positionRepository = positionRepository;
            _openPositionRepository = openPositionRepository;
            _summaryReportService = summaryReportService;
            _instrumentService = instrumentService;
            _quoteService = quoteService;
            _rateService = rateService;
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

            decimal avgPrice = internalTrades.Sum(o => o.Price) / internalTrades.Count;

            decimal volume = internalTrades.Sum(o => o.Volume);

            Position position;

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

                decimal price = tradeType == TradeType.Sell
                    ? Calculator.CalculateDirectSellPrice(avgPrice, quote, crossInstrument.IsInverse)
                    : Calculator.CalculateDirectBuyPrice(avgPrice, quote, crossInstrument.IsInverse);

                decimal? priceUsd = await _rateService.CalculatePriceInUsd(instrument.AssetPairId, price);

                position = Position.Open(instrument.AssetPairId, price, priceUsd, avgPrice, volume, quote,
                    crossInstrument.AssetPairId, tradeType, internalTrades.Select(o => o.Id).ToArray());
            }
            else
            {
                decimal? avgPriceUsd = await _rateService.CalculatePriceInUsd(instrument.AssetPairId, avgPrice);

                position = Position.Open(instrument.AssetPairId, avgPrice, avgPriceUsd, volume, tradeType,
                    internalTrades.Select(o => o.Id).ToArray());
            }

            await _openPositionRepository.InsertAsync(position);

            await _positionRepository.InsertAsync(position);

            await _summaryReportService.RegisterOpenPositionAsync(position, internalTrades);

            _log.InfoWithDetails("Position was opened", position);
        }

        public async Task CloseAsync(Position position, ExternalTrade externalTrade)
        {
            decimal? priceUsd = await _rateService.CalculatePriceInUsd(position.AssetPairId, externalTrade.Price);

            position.Close(externalTrade.Id, externalTrade.Price, priceUsd);

            await _positionRepository.UpdateAsync(position);

            await _openPositionRepository.DeleteAsync(position.AssetPairId, position.Id);

            await _summaryReportService.RegisterClosePositionAsync(position);

            _log.InfoWithDetails("Position was closed", position);
        }

        public async Task CloseRemainingVolumeAsync(string assetPairId, ExternalTrade externalTrade)
        {
            decimal? priceUsd = await _rateService.CalculatePriceInUsd(assetPairId, externalTrade.Price);

            Position position = Position.Create(assetPairId, externalTrade.Id, externalTrade.Type, externalTrade.Price,
                priceUsd, externalTrade.Volume);

            await _positionRepository.InsertAsync(position);

            _log.InfoWithDetails("Position with remaining volume was closed", position);
        }
    }
}
