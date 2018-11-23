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
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    [ResponseCache(Duration = 5)]
    public class ReportsController : Controller, IReportsApi
    {
        private readonly IAssetSettingsService _assetSettingsService;
        private readonly IBalanceService _balanceService;
        private readonly ICreditService _creditService;
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;
        private readonly ISummaryReportService _summaryReportService;
        private readonly IPositionReportService _positionReportService;
        private readonly IAssetsServiceWithCache _lykkeAssetService;

        public ReportsController(
            IAssetSettingsService assetSettingsService,
            IBalanceService balanceService,
            ICreditService creditService,
            ICrossRateInstrumentService crossRateInstrumentService,
            ISummaryReportService summaryReportService,
            IPositionReportService positionReportService,
            IAssetsServiceWithCache lykkeAssetService)
        {
            _assetSettingsService = assetSettingsService;
            _balanceService = balanceService;
            _creditService = creditService;
            _crossRateInstrumentService = crossRateInstrumentService;
            _summaryReportService = summaryReportService;
            _positionReportService = positionReportService;
            _lykkeAssetService = lykkeAssetService;
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

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summaryByPeriod")]
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
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionReportModel>> GetPositionsReportAsync(DateTime startDate,
            DateTime endDate, int limit)
        {
            IReadOnlyCollection<PositionReport> positionReports =
                await _positionReportService.GetByPeriodAsync(startDate, endDate, limit);

            return Mapper.Map<PositionReportModel[]>(positionReports);
        }

        [HttpGet("balances")]
        [ProducesResponseType(typeof(IReadOnlyCollection<BalanceReportModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<BalanceReportModel>> GetBalancesReportAsync()
        {
            IDictionary<string, Balance> lykkeBalances = (await _balanceService.GetAsync(ExchangeNames.Lykke))
                .ToDictionary(o => o.AssetId, o => o);
            IDictionary<string, Credit> lykkeCredits = (await _creditService.GetAllAsync())
                .ToDictionary(o => o.AssetId, o => o);
            IReadOnlyCollection<Balance> externalBalances = await _balanceService.GetAsync(ExchangeNames.External);
            IReadOnlyCollection<AssetSettings> assetSettings = await _assetSettingsService.GetAllAsync();

            string[] lykkeAssets = lykkeBalances.Keys
                .Union(lykkeCredits.Keys)
                .ToArray();

            var model = new List<BalanceReportModel>();

            foreach (string assetId in lykkeAssets)
            {
                var asset = await _lykkeAssetService.TryGetAssetAsync(assetId);
                AssetSettings settings = assetSettings.FirstOrDefault(o => o.LykkeAssetId == assetId);
                lykkeBalances.TryGetValue(assetId, out Balance balance);
                lykkeCredits.TryGetValue(assetId, out Credit credit);

                decimal balanceAmount = balance?.Amount ?? decimal.Zero;
                decimal creditAmount = credit?.Amount ?? decimal.Zero;

                model.Add(new BalanceReportModel
                {
                    Asset = asset?.DisplayId ?? assetId,
                    LykkeAssetId = assetId,
                    LykkeAmount = balanceAmount,
                    LykkeCreditAmount = creditAmount,
                    ExternalAssetId = settings?.AssetId
                });
            }

            foreach (Balance externalBalance in externalBalances)
            {
                model.Add(new BalanceReportModel
                {
                    Asset = externalBalance.AssetId,
                    ExternalAssetId = externalBalance.AssetId,
                    ExternalAmount = externalBalance.Amount
                });
            }

            IReadOnlyCollection<BalanceReportModel> rows = model
                .GroupBy(o => o.ExternalAssetId)
                .Where(g => g.Key != null)
                .Select(g =>
                {
                    BalanceReportModel lykkeBalance = g.FirstOrDefault(o => o.LykkeAssetId != null);
                    BalanceReportModel externalBalance = g.FirstOrDefault(o => o.ExternalAmount != null);

                    return new BalanceReportModel
                    {
                        Asset = g.Key,
                        LykkeAssetId = lykkeBalance?.LykkeAssetId,
                        LykkeAmount = lykkeBalance?.LykkeAmount,
                        LykkeCreditAmount = lykkeBalance?.LykkeCreditAmount,
                        ExternalAssetId = externalBalance?.ExternalAssetId,
                        ExternalAmount = externalBalance?.ExternalAmount
                    };
                })
                .Concat(model.Where(o => o.ExternalAssetId == null))
                .ToArray();

            foreach (BalanceReportModel balance in rows)
            {
                decimal lykkeBalance = (balance.LykkeAmount ?? 0) - (balance.LykkeCreditAmount ?? 0);
                decimal totalAmount = lykkeBalance + (balance.ExternalAmount ?? 0);
                (decimal? totalAmountInUsd, decimal? rate)
                    = balance.ExternalAssetId != null
                        ? await _assetSettingsService.ConvertAmountAsync(balance.ExternalAssetId, totalAmount)
                        : (null, null);

                balance.TotalAmount = totalAmount;
                balance.TotalAmountInUsd = totalAmountInUsd;
                balance.CrossRate = rate;
            }

            return rows
                .Where(o => o.TotalAmount != 0)
                .ToArray();
        }
    }
}
