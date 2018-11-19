using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lykke.Service.Assets.Client;
using Lykke.Service.Assets.Client.Models;

namespace Lykke.Service.LiquidityEngine.DomainServices.CrossRateInstruments
{
    public class CrossRateInstrumentService : ICrossRateInstrumentService
    {
        public const string UsdAssetId = "USD";

        private readonly ICrossRateInstrumentRepository _crossRateInstrumentRepository;
        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IQuoteService _quoteService;
        private readonly InMemoryCache<CrossRateInstrument> _cache;
        private readonly ILog _log;

        public CrossRateInstrumentService(
            ICrossRateInstrumentRepository crossRateInstrumentRepository,
            IAssetsServiceWithCache assetsServiceWithCache,
            IQuoteService quoteService,
            ILogFactory logFactory)
        {
            _crossRateInstrumentRepository = crossRateInstrumentRepository;
            _assetsServiceWithCache = assetsServiceWithCache;
            _quoteService = quoteService;
            _cache = new InMemoryCache<CrossRateInstrument>(instrument => instrument.AssetPairId, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<CrossRateInstrument>> GetAllAsync()
        {
            IReadOnlyCollection<CrossRateInstrument> crossInstruments = _cache.GetAll();

            if (crossInstruments == null)
            {
                crossInstruments = await _crossRateInstrumentRepository.GetAllAsync();

                _cache.Initialize(crossInstruments);
            }

            return crossInstruments;
        }

        public async Task<CrossRateInstrument> GetByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<CrossRateInstrument> crossInstruments = await GetAllAsync();

            CrossRateInstrument crossInstrument = crossInstruments.SingleOrDefault(o => o.AssetPairId == assetPairId);

            if (crossInstrument == null)
                throw new EntityNotFoundException();

            return crossInstrument;
        }

        public async Task AddAsync(CrossRateInstrument crossInstrument)
        {
            await _crossRateInstrumentRepository.InsertAsync(crossInstrument);

            _cache.Set(crossInstrument);

            _log.InfoWithDetails("Cross-rate instrument was added", crossInstrument);
        }

        public async Task UpdateAsync(CrossRateInstrument crossInstrument)
        {
            CrossRateInstrument currentCrossInstrument = await GetByAssetPairIdAsync(crossInstrument.AssetPairId);

            currentCrossInstrument.Update(crossInstrument);

            await _crossRateInstrumentRepository.UpdateAsync(currentCrossInstrument);

            _cache.Set(currentCrossInstrument);

            _log.InfoWithDetails("Cross-rate instrument was updated", currentCrossInstrument);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            CrossRateInstrument crossInstrument = await GetByAssetPairIdAsync(assetPairId);

            await _crossRateInstrumentRepository.DeleteAsync(assetPairId);

            _cache.Remove(assetPairId);

            _log.InfoWithDetails("Cross-rate instrument was deleted", crossInstrument);
        }

        public async Task<decimal?> ConvertPriceAsync(string assetPairId, decimal price)
        {
            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            if (assetPair.QuotingAssetId == UsdAssetId)
                return price;
            
            IReadOnlyCollection<CrossRateInstrument> crossInstruments = await GetAllAsync();

            CrossRateInstrument crossInstrument = crossInstruments.SingleOrDefault(o => o.AssetPairId == assetPairId);

            if (crossInstrument == null)
            {
                _log.WarningWithDetails("Cross instrument not found", assetPairId);
                return null;
            }

            Quote quote = await _quoteService
                .GetAsync(crossInstrument.QuoteSource, crossInstrument.ExternalAssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for cross instrument", crossInstrument);
                return null;
            }

            (decimal crossPrice, decimal _) =
                Calculator.CalculateCrossMidPrice(price, quote, crossInstrument.IsInverse);

            return crossPrice;
        }
    }
}
