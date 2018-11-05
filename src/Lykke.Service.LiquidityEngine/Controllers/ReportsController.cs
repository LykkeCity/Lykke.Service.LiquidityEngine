using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Reports;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    [ResponseCache(Duration = 5)]
    public class ReportsController : Controller, IReportsApi
    {
        private readonly ISummaryReportService _summaryReportService;
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;

        public ReportsController(
            ISummaryReportService summaryReportService,
            ICrossRateInstrumentService crossRateInstrumentService)
        {
            _summaryReportService = summaryReportService;
            _crossRateInstrumentService = crossRateInstrumentService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportAsync()
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportService.GetAllAsync();

            var model = Mapper.Map<SummaryReportModel[]>(summaryReports);

            foreach (SummaryReportModel summaryReportModel in model)
            {
                summaryReportModel.PnLUsd = await _crossRateInstrumentService.ConvertPriceAsync(
                    summaryReportModel.AssetPairId, summaryReportModel.PnL);
            }

            return model;
        }
    }
}
