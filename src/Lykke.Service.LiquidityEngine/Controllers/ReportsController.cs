using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.Assets.Client;
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
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;
        private readonly IInstrumentService _instrumentService;
        private readonly IPositionService _positionService;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IPositionReportService _positionReportService;

        public ReportsController(
            IBalanceIndicatorsReportService balanceIndicatorsReportService,
            IBalanceReportService balanceReportService,
            ICrossRateInstrumentService crossRateInstrumentService,
            IInstrumentService instrumentService,
            IPositionService positionService,
            ISummaryReportService summaryReportService,
            IPositionReportService positionReportService,
            IAssetsServiceWithCache lykkeAssetService)
        {
            _balanceIndicatorsReportService = balanceIndicatorsReportService;
            _balanceReportService = balanceReportService;
            _crossRateInstrumentService = crossRateInstrumentService;
            _instrumentService = instrumentService;
            _positionService = positionService;
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
            IReadOnlyCollection<SummaryReport> summaryReports = await _summaryReportService.GetAllAsync();

            var model = Mapper.Map<SummaryReportModel[]>(summaryReports);

            await ExtendSummaryReportAsync(model);

            return model;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summaryByPeriod")]
        [ResponseCache(Duration = 5)]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportByPeriodAsync(DateTime startDate,
            DateTime endDate)
        {
            IReadOnlyCollection<SummaryReport> summaryReports =
                await _summaryReportService.GetByPeriodAsync(startDate, endDate);

            var model = Mapper.Map<SummaryReportModel[]>(summaryReports);

            foreach (SummaryReportModel summaryReportModel in model)
            {
                summaryReportModel.PnLUsd = await _crossRateInstrumentService.ConvertPriceAsync(
                    summaryReportModel.AssetPairId, summaryReportModel.PnL);
            }

            return model;
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

        private async Task ExtendSummaryReportAsync(IReadOnlyCollection<SummaryReportModel> summaryReport)
        {
            IReadOnlyCollection<Position> openPositions = await _positionService.GetOpenAllAsync();
            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            foreach (SummaryReportModel summaryReportModel in summaryReport)
            {
                Instrument instrument =
                    instruments.FirstOrDefault(o => o.AssetPairId == summaryReportModel.AssetPairId);

                decimal openVolume = openPositions
                    .Where(o => o.AssetPairId == summaryReportModel.AssetPairId &&
                                o.TradeAssetPairId == summaryReportModel.TradeAssetPairId)
                    .Sum(o => Math.Abs(o.Volume));

                decimal? pnlUsd = await _crossRateInstrumentService.ConvertPriceAsync(
                    summaryReportModel.AssetPairId, summaryReportModel.PnL);

                summaryReportModel.OpenVolume = openVolume;
                summaryReportModel.OpenVolumeLimit = openVolume / instrument?.InventoryThreshold;
                summaryReportModel.PnLUsd = pnlUsd;
            }
        }
    }
}
