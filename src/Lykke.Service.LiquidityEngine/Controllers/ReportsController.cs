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
    public class ReportsController : Controller, IReportsApi
    {
        private readonly ISummaryReportService _summaryReportService;

        public ReportsController(ISummaryReportService summaryReportService)
        {
            _summaryReportService = summaryReportService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportAsync()
        {
            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportService.GetAllAsync();

            return Mapper.Map<SummaryReportModel[]>(summaryReports);
        }
    }
}
