using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.InstrumentMarkups
{
    public class InstrumentMarkupService : IInstrumentMarkupService
    {
        private readonly IMarketMakerSettingsService _marketMakerSettingsService;
        private readonly IPnLStopLossEngineService _pnLStopLossEngineService;
        private readonly IFiatEquityStopLossService _fiatEquityStopLossService;
        private readonly INoFreshQuotesStopLossService _noFreshQuotesStopLossService;
        private readonly IInstrumentService _instrumentService;

        public InstrumentMarkupService(
            IMarketMakerSettingsService marketMakerSettingsService,
            IPnLStopLossEngineService pnLStopLossEngineService,
            IFiatEquityStopLossService fiatEquityStopLossService,
            INoFreshQuotesStopLossService noFreshQuotesStopLossService,
            IInstrumentService instrumentService)
        {
            _marketMakerSettingsService = marketMakerSettingsService;
            _pnLStopLossEngineService = pnLStopLossEngineService;
            _fiatEquityStopLossService = fiatEquityStopLossService;
            _noFreshQuotesStopLossService = noFreshQuotesStopLossService;
            _instrumentService = instrumentService;
        }

        public async Task<IReadOnlyCollection<AssetPairMarkup>> GetAllAsync()
        {
            List<AssetPairMarkup> result = new List<AssetPairMarkup>();

            decimal globalMarkup = (await _marketMakerSettingsService.GetAsync()).LimitOrderPriceMarkup;

            IReadOnlyCollection<AssetPairMarkup> pnLStopLossMarkups = await _pnLStopLossEngineService.GetTotalMarkups();

            IReadOnlyCollection<AssetPairMarkup> fiatEquityStopLossMarkups = await _fiatEquityStopLossService.GetMarkupsAsync();

            IReadOnlyCollection<AssetPairMarkup> noFreshQuotesStopLossMarkups = await _noFreshQuotesStopLossService.GetMarkupsAsync();

            var assetPairs = (await _instrumentService.GetAllAsync()).Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairs)
            {
                AssetPairMarkup assetPairMarkup = new AssetPairMarkup();

                assetPairMarkup.AssetPairId = assetPairId;

                // Global
                assetPairMarkup.TotalMarkup += globalMarkup;
                assetPairMarkup.TotalAskMarkup += globalMarkup;
                assetPairMarkup.TotalBidMarkup += globalMarkup;

                // PnL Stop Loss Markups
                AssetPairMarkup pnLStopLossMarkup = pnLStopLossMarkups.SingleOrDefault(x => x.AssetPairId == assetPairId);
                if (pnLStopLossMarkup != null)
                {
                    assetPairMarkup.TotalMarkup += pnLStopLossMarkup.TotalMarkup;
                    assetPairMarkup.TotalAskMarkup += pnLStopLossMarkup.TotalAskMarkup;
                    assetPairMarkup.TotalBidMarkup += pnLStopLossMarkup.TotalBidMarkup;
                }

                // Fiat Equity Stop Loss Markups
                AssetPairMarkup fiatEquityStopLossMarkup = fiatEquityStopLossMarkups.SingleOrDefault(x => x.AssetPairId == assetPairId);
                if (fiatEquityStopLossMarkup != null)
                {
                    // Can be applied to asks only
                    if (fiatEquityStopLossMarkup.TotalAskMarkup == decimal.MinusOne)
                        assetPairMarkup.TotalAskMarkup = decimal.MinusOne;
                    else
                        assetPairMarkup.TotalAskMarkup += fiatEquityStopLossMarkup.TotalAskMarkup;
                }

                // No Fresh Quotes Stop Loss Markups
                AssetPairMarkup noFreshQuotesStopLossMarkup = noFreshQuotesStopLossMarkups.SingleOrDefault(x => x.AssetPairId == assetPairId);
                if (noFreshQuotesStopLossMarkup != null)
                {
                    assetPairMarkup.TotalMarkup += noFreshQuotesStopLossMarkup.TotalMarkup;
                    assetPairMarkup.TotalAskMarkup += noFreshQuotesStopLossMarkup.TotalAskMarkup;
                    assetPairMarkup.TotalBidMarkup += noFreshQuotesStopLossMarkup.TotalBidMarkup;
                }

                result.Add(assetPairMarkup);
            }

            return result;
        }
    }
}
