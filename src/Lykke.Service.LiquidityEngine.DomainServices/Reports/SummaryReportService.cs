using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly InMemoryCache<SummaryReport> _cache;
        private readonly ILog _log;

        public SummaryReportService(ISummaryReportRepository summaryReportRepository, ILogFactory logFactory)
        {
            _summaryReportRepository = summaryReportRepository;
            _cache = new InMemoryCache<SummaryReport>(summaryReport => summaryReport.AssetPairId, false);
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
    }
}
