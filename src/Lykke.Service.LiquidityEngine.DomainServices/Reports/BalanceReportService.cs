using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.Reports
{
    public class BalanceReportService : IBalanceReportService
    {
        private readonly ICreditService _creditService;
        private readonly IBalanceService _balanceService;
        private readonly IAssetSettingsService _assetSettingsService;
        private readonly IQuoteService _quoteService;

        public BalanceReportService(
            ICreditService creditService,
            IBalanceService balanceService,
            IAssetSettingsService assetSettingsService,
            IQuoteService quoteService)
        {
            _creditService = creditService;
            _balanceService = balanceService;
            _assetSettingsService = assetSettingsService;
            _quoteService = quoteService;
        }
        
        public async Task<IReadOnlyCollection<BalanceReport>> GetAsync()
        {
            IReadOnlyCollection<Balance> lykkeBalances = await _balanceService.GetAsync(ExchangeNames.Lykke);
            
            IReadOnlyCollection<Balance> externalBalances = await _balanceService.GetAsync(ExchangeNames.External);

            IReadOnlyCollection<Credit> credits = await _creditService.GetAllAsync();
            
            IReadOnlyCollection<Domain.AssetSettings> assetsSettings = await _assetSettingsService.GetAllAsync();

            string[] assets = externalBalances
                .Select(o => o.AssetId)
                .Union(lykkeBalances.Select(o => o.AssetId).Union(credits.Select(o => o.AssetId))
                    .Select(o => assetsSettings.SingleOrDefault(p => p.LykkeAssetId == o)?.AssetId ?? o))
                .ToArray();

            var reports = new List<BalanceReport>();

            foreach (string assetId in assets)
            {
                Domain.AssetSettings assetSettings = assetsSettings.SingleOrDefault(o => o.AssetId == assetId);

                string lykkeAssetId = assetSettings?.LykkeAssetId ?? assetId;

                Balance lykkeBalance = lykkeBalances.SingleOrDefault(o => o.AssetId == lykkeAssetId);

                Balance externalBalance = externalBalances.SingleOrDefault(o => o.AssetId == assetId);

                Credit credit = credits.SingleOrDefault(o => o.AssetId == lykkeAssetId);

                decimal lykkeDisbalance = (lykkeBalance?.Amount ?? 0) - (credit?.Amount ?? 0);
                
                decimal totalAmount = lykkeDisbalance + (externalBalance?.Amount ?? 0);

                if (totalAmount != 0)
                {
                    (decimal? TotalAmountInUsd, decimal? Rate) tuple = (null, null);

                    if (assetId == AssetConsts.UsdAssetId)
                    {
                        tuple = (totalAmount, 1);
                    }
                    else if (assetSettings != null)
                    {
                        Quote quote = await _quoteService
                            .GetAsync(assetSettings.QuoteSource, assetSettings.ExternalAssetPairId);

                        if (quote != null)
                            tuple = Calculator.CalculateCrossMidPrice(totalAmount, quote, assetSettings.IsInverse);
                    }

                    reports.Add(new BalanceReport
                    {
                        Asset = assetId,
                        LykkeAssetId = lykkeAssetId,
                        ExternalAssetId = assetId,
                        LykkeAmount = lykkeBalance?.Amount,
                        LykkeCreditAmount = credit?.Amount,
                        LykkeDisbalance = lykkeDisbalance,
                        ExternalAmount = externalBalance?.Amount,
                        TotalAmount = totalAmount,
                        TotalAmountInUsd = tuple.TotalAmountInUsd,
                        CrossRate = tuple.Rate
                    });
                }
            }

            return reports;
        }
    }
}
