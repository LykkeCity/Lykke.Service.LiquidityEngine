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
        private readonly InMemoryCache<SummaryReport> _cache;
        private readonly ILog _log;

        public SummaryReportService(
            ISummaryReportRepository summaryReportRepository,
            [KeyFilter("PositionRepositoryPostgres")] IPositionRepository positionRepositoryPostgres,
            ILogFactory logFactory)
        {
            _summaryReportRepository = summaryReportRepository;
            _positionRepositoryPostgres = positionRepositoryPostgres;
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

        public Task<IReadOnlyCollection<SummaryReport>> GetByPeriodAsync(DateTime startDate, DateTime endDate)
        {
            return _positionRepositoryPostgres.GetReportAsync(startDate, endDate);
        }

        public async Task RegisterOpenPositionAsync(Position position, IReadOnlyCollection<InternalTrade> internalTrades)
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

        private static string CacheKey(SummaryReport summaryReport)
        {
            return $"{summaryReport.AssetPairId}_{summaryReport.TradeAssetPairId}";
        }
    }
}
