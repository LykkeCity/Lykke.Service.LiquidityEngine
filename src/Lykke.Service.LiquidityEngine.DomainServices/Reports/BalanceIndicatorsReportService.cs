using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Reports
{
    public class BalanceIndicatorsReportService : IBalanceIndicatorsReportService
    {
        private readonly IAssetSettingsService _assetSettingsService;
        private readonly IQuoteService _quoteService;
        private readonly IBalanceService _balanceService;

        public BalanceIndicatorsReportService(
            IAssetSettingsService assetSettingsService,
            IQuoteService quoteService,
            IBalanceService balanceService)
        {
            _assetSettingsService = assetSettingsService;
            _quoteService = quoteService;
            _balanceService = balanceService;
        }

        public async Task<BalanceIndicatorsReport> GetAsync()
        {
            IReadOnlyCollection<Balance> externalBalances = await _balanceService.GetAsync(ExchangeNames.External);

            decimal riskExposure = 0;
            decimal equity = 0;

            foreach (Balance balance in externalBalances)
            {
                decimal amountInUsd;

                if (balance.AssetId == AssetConsts.UsdAssetId)
                {
                    amountInUsd = balance.Amount;
                }
                else
                {

                    Domain.AssetSettings assetSettings = await _assetSettingsService.GetByIdAsync(balance.AssetId);

                    if (assetSettings == null)
                        continue;

                    Quote quote = await _quoteService
                        .GetAsync(assetSettings.QuoteSource, assetSettings.ExternalAssetPairId);

                    if (quote == null)
                        continue;

                    amountInUsd = Calculator.CalculateCrossMidPrice(balance.Amount, quote, assetSettings.IsInverse)
                        .Item1;
                }

                if (balance.Amount < 0)
                    riskExposure += Math.Abs(amountInUsd);

                equity += amountInUsd;
            }

            return new BalanceIndicatorsReport
            {
                Equity = equity,
                RiskExposure = riskExposure
            };
        }
    }
}
