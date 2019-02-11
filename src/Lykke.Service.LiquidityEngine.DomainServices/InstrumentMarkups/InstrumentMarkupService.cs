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
        private readonly IInstrumentService _instrumentService;

        public InstrumentMarkupService(
            IMarketMakerSettingsService marketMakerSettingsService,
            IPnLStopLossEngineService pnLStopLossEngineService,
            IFiatEquityStopLossService fiatEquityStopLossService,
            IInstrumentService instrumentService)
        {
            _marketMakerSettingsService = marketMakerSettingsService;
            _pnLStopLossEngineService = pnLStopLossEngineService;
            _fiatEquityStopLossService = fiatEquityStopLossService;
            _instrumentService = instrumentService;
        }

        public async Task<IReadOnlyCollection<AssetPairMarkup>> GetAllAsync()
        {
            List<AssetPairMarkup> result = new List<AssetPairMarkup>();

            decimal globalMarkup = (await _marketMakerSettingsService.GetAsync()).LimitOrderPriceMarkup;

            IReadOnlyCollection<AssetPairMarkup> pnLStopLossMarkups = await _pnLStopLossEngineService.GetTotalMarkups();

            IReadOnlyCollection<AssetPairMarkup> fiatStopLossMarkups = await _fiatEquityStopLossService.GetMarkups();

            var assetPairs = (await _instrumentService.GetAllAsync()).Select(x => x.AssetPairId).ToList();

            foreach (var assetPairId in assetPairs)
            {
                AssetPairMarkup assetPairMarkup = new AssetPairMarkup();

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
                AssetPairMarkup fiatEquityStopLossMarkup = fiatStopLossMarkups.SingleOrDefault(x => x.AssetPairId == assetPairId);
                if (fiatEquityStopLossMarkup != null)
                {
                    // Can be applied to asks only
                    assetPairMarkup.TotalAskMarkup += fiatEquityStopLossMarkup.TotalAskMarkup;
                }

                result.Add(assetPairMarkup);
            }

            return result;
        }
    }
}
