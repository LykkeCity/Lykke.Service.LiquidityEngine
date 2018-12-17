using System;
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
        private readonly IBalanceIndicatorsReportService _balanceIndicatorsReportService;
        private readonly IBalanceReportService _balanceReportService;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IPositionReportService _positionReportService;

        public ReportsController(
            IBalanceIndicatorsReportService balanceIndicatorsReportService,
            IBalanceReportService balanceReportService,
            ISummaryReportService summaryReportService,
            IPositionReportService positionReportService)
        {
            _balanceIndicatorsReportService = balanceIndicatorsReportService;
            _balanceReportService = balanceReportService;
            _summaryReportService = summaryReportService;
            _positionReportService = positionReportService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summary")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportAsync()
        {
            IReadOnlyCollection<PositionSummaryReport> reports = await _summaryReportService.GetAsync();

            return Mapper.Map<SummaryReportModel[]>(reports);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summaryByPeriod")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportByPeriodAsync(DateTime startDate,
            DateTime endDate)
        {
            IReadOnlyCollection<PositionSummaryReport> reports =
                await _summaryReportService.GetByPeriodAsync(startDate, endDate);

            return Mapper.Map<SummaryReportModel[]>(reports);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions reports.</response>
        [HttpGet("positions")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionReportModel>> GetPositionsReportAsync(DateTime startDate,
            DateTime endDate, int limit)
        {
            IReadOnlyCollection<PositionReport> positionReports =
                await _positionReportService.GetByPeriodAsync(startDate, endDate, limit);

            return Mapper.Map<PositionReportModel[]>(positionReports);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset balance info.</response>
        [HttpGet("balances")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(IReadOnlyCollection<BalanceReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<BalanceReportModel>> GetBalancesReportAsync()
        {
            IReadOnlyCollection<BalanceReport> balanceReports = await _balanceReportService.GetAsync();

            return Mapper.Map<BalanceReportModel[]>(balanceReports);
        }

        /// <inheritdoc/>
        /// <response code="200">A balance indicators report.</response>
        [HttpGet("balanceindicators")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(BalanceIndicatorsReportModel), (int) HttpStatusCode.OK)]
        public async Task<BalanceIndicatorsReportModel> GetBalanceIndicatorsReportAsync()
        {
            BalanceIndicatorsReport balanceIndicatorsReport = await _balanceIndicatorsReportService.GetAsync();

            return Mapper.Map<BalanceIndicatorsReportModel>(balanceIndicatorsReport);
        }
    }
}
