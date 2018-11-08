using System.Collections.Generic;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;

namespace Lykke.Service.LiquidityEngine.Migration.Operations
{
    [UsedImplicitly]
    public class MigrateSummaryReportOperation : IMigrationOperation
    {
        private readonly ISummaryReportRepository _summaryReportRepository;
        private readonly ILog _log;

        public int ApplyToVersion => 0;

        public int UpdatedVersion => 1;

        public MigrateSummaryReportOperation(
            ISummaryReportRepository summaryReportRepository,
            ILogFactory logFactory)
        {
            _summaryReportRepository = summaryReportRepository;
            _log = logFactory.CreateLog(this);
        }

        public async Task ApplyAsync()
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportRepository.GetAllAsync();

            _log.InfoWithDetails("Read summary reports", new
            {
                summaryReports.Count
            });

            await _summaryReportRepository.DeleteAsync();

            foreach (SummaryReport summaryReport in summaryReports)
            {
                summaryReport.TradeAssetPairId = summaryReport.AssetPairId;

                await _summaryReportRepository.InsertAsync(summaryReport);

                _log.InfoWithDetails("Updated summary report.", new
                {
                    SummaryReport = summaryReport
                });
            }
        }
    }
}
