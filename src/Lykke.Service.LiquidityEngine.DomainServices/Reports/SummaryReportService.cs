using System;
using System.Collections.Generic;
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

namespace Lykke.Service.LiquidityEngine.DomainServices.Reports
{
    [UsedImplicitly]
    public class SummaryReportService : ISummaryReportService
    {
        private readonly ISummaryReportRepository _summaryReportRepository;
        private readonly IPositionRepository _positionRepositoryPostgres;
        private readonly IOpenPositionRepository _openPositionRepository;
        private readonly IInstrumentService _instrumentService;
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;
        private readonly InMemoryCache<SummaryReport> _cache;
        private readonly ILog _log;

        public SummaryReportService(
            ISummaryReportRepository summaryReportRepository,
            [KeyFilter("PositionRepositoryPostgres")] IPositionRepository positionRepositoryPostgres,
            IOpenPositionRepository openPositionRepository,
            IInstrumentService instrumentService,
            ICrossRateInstrumentService crossRateInstrumentService,
            ILogFactory logFactory)
        {
            _summaryReportRepository = summaryReportRepository;
            _positionRepositoryPostgres = positionRepositoryPostgres;
            _openPositionRepository = openPositionRepository;
            _instrumentService = instrumentService;
            _crossRateInstrumentService = crossRateInstrumentService;
            _cache = new InMemoryCache<SummaryReport>(CacheKey, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<SummaryReport>> GetAllAsync()
        {
            IReadOnlyCollection<SummaryReport> summaryReports = _cache.GetAll();

            if (summaryReports == null)
            {
                summaryReports = await _summaryReportRepository.GetAllAsync();

                _cache.Initialize(summaryReports);
            }

            return summaryReports;
        }

        public async Task<IReadOnlyCollection<PositionSummaryReport>> GetAsync()
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await GetAllAsync();

            return await ConvertToReportsAsync(summaryReports);
        }

        public async Task<IReadOnlyCollection<PositionSummaryReport>> GetByPeriodAsync(DateTime startDate,
            DateTime endDate)
        {
            IReadOnlyCollection<SummaryReport> summaryReports =
                await _positionRepositoryPostgres.GetReportAsync(startDate, endDate);
            
            return await ConvertToReportsAsync(summaryReports);
        }

        public async Task RegisterOpenPositionAsync(Position position,
            IReadOnlyCollection<InternalTrade> internalTrades)
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await GetAllAsync();

            SummaryReport summaryReport = summaryReports.SingleOrDefault(o =>
                o.AssetPairId == position.AssetPairId && o.TradeAssetPairId == position.TradeAssetPairId);

            bool isNew = false;

            if (summaryReport == null)
            {
                summaryReport = new SummaryReport
                {
                    AssetPairId = position.AssetPairId,
                    TradeAssetPairId = position.TradeAssetPairId
                };
                isNew = true;
            }

            foreach (InternalTrade internalTrade in internalTrades)
                summaryReport.ApplyTrade(internalTrade);

            summaryReport.ApplyOpenPosition();

            if (isNew)
                await _summaryReportRepository.InsertAsync(summaryReport);
            else
                await _summaryReportRepository.UpdateAsync(summaryReport);

            _cache.Set(summaryReport);
        }

        public async Task RegisterClosePositionAsync(Position position)
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await GetAllAsync();

            SummaryReport summaryReport = summaryReports.SingleOrDefault(o =>
                o.AssetPairId == position.AssetPairId && o.TradeAssetPairId == position.TradeAssetPairId);

            if (summaryReport == null)
            {
                _log.WarningWithDetails("Summary report does not exist", position);
                return;
            }

            summaryReport.ApplyClosePosition(position);

            await _summaryReportRepository.UpdateAsync(summaryReport);

            _cache.Set(summaryReport);
        }

        private async Task<IReadOnlyCollection<PositionSummaryReport>> ConvertToReportsAsync(
            IEnumerable<SummaryReport> summaryReports)
        {
            IReadOnlyCollection<Position> openPositions = await _openPositionRepository.GetAllAsync();

            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            var items = new List<PositionSummaryReport>();

            foreach (SummaryReport summaryReport in summaryReports)
            {
                Instrument instrument = instruments.FirstOrDefault(o => o.AssetPairId == summaryReport.AssetPairId);

                decimal openVolume = openPositions
                    .Where(o => o.AssetPairId == summaryReport.AssetPairId &&
                                o.TradeAssetPairId == summaryReport.TradeAssetPairId)
                    .Sum(o => Math.Abs(o.Volume));

                decimal? pnlUsd = await _crossRateInstrumentService.ConvertPriceAsync(summaryReport.AssetPairId,
                    summaryReport.PnL);

                items.Add(new PositionSummaryReport
                {
                    AssetPairId = summaryReport.AssetPairId,
                    TradeAssetPairId = summaryReport.TradeAssetPairId,
                    OpenPositionsCount = summaryReport.OpenPositionsCount,
                    ClosedPositionsCount = summaryReport.ClosedPositionsCount,
                    PnL = summaryReport.PnL,
                    PnLUsd = pnlUsd,
                    BaseAssetVolume = summaryReport.BaseAssetVolume,
                    QuoteAssetVolume = summaryReport.QuoteAssetVolume,
                    TotalSellBaseAssetVolume = summaryReport.TotalSellBaseAssetVolume,
                    TotalBuyBaseAssetVolume = summaryReport.TotalBuyBaseAssetVolume,
                    TotalSellQuoteAssetVolume = summaryReport.TotalSellQuoteAssetVolume,
                    TotalBuyQuoteAssetVolume = summaryReport.TotalBuyQuoteAssetVolume,
                    SellTradesCount = summaryReport.SellTradesCount,
                    BuyTradesCount = summaryReport.BuyTradesCount,
                    OpenVolume = openVolume,
                    OpenVolumeLimit = openVolume / instrument?.InventoryThreshold
                });
            }

            return items;
        }

        private static string CacheKey(SummaryReport summaryReport)
        {
            return $"{summaryReport.AssetPairId}_{summaryReport.TradeAssetPairId}";
        }
    }
}
