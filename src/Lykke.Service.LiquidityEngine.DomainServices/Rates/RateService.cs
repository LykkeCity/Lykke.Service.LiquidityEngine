using System;
using System.Threading.Tasks;
using Common.Cache;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.Assets.Client;
using Lykke.Service.LiquidityEngine.Domain.Consts;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Lykke.Service.RateCalculator.Client;
using AssetPair = Lykke.Service.Assets.Client.Models.AssetPair;

namespace Lykke.Service.LiquidityEngine.DomainServices.Rates
{
    [UsedImplicitly]
    public class RateService : IRateService
    {
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(1);

        private readonly IAssetsServiceWithCache _assetsServiceWithCache;
        private readonly IRateCalculatorClient _rateCalculatorClient;
        private readonly ICacheManager _cache;
        private readonly ILog _log;

        public RateService(
            IAssetsServiceWithCache assetsServiceWithCache,
            IRateCalculatorClient rateCalculatorClient,
            ICacheManager cache,
            ILogFactory logFactory)
        {
            _assetsServiceWithCache = assetsServiceWithCache;
            _rateCalculatorClient = rateCalculatorClient;
            _cache = cache;
            _log = logFactory.CreateLog(this);
        }


        public async Task<decimal> GetQuotingToUsdRate(string assetPairId)
        {
            AssetPair assetPair = await _assetsServiceWithCache.TryGetAssetPairAsync(assetPairId);

            if (assetPair == null)
            {
                _log.WarningWithDetails("Asset pair is not found.", new
                {
                    AssetPairId = assetPairId
                });
                return decimal.Zero;
            }

            if (assetPair.QuotingAssetId == AssetConsts.UsdAssetId)
            {
                return 1;
            }

            AssetRate assetRate = _cache.Get<AssetRate>(GetCacheKey(assetPair.QuotingAssetId));

            if (assetRate == null)
            {
                decimal rate = await GetRate(assetPair.QuotingAssetId, AssetConsts.UsdAssetId);

                assetRate = new AssetRate
                {
                    AssetIdFrom = assetPair.QuotingAssetId,
                    AssetIdTo = AssetConsts.UsdAssetId,
                    Rate = (decimal) rate
                };

                _cache.Set(GetCacheKey(assetPair.QuotingAssetId), assetRate, (int) CacheDuration.TotalMinutes);
            }

            return assetRate.Rate;
        }

        private async Task<decimal> GetRate(string assetIdFrom, string assetIdTo)
        {
            decimal rate = 0;

            try
            {
                rate = (decimal) await _rateCalculatorClient.GetAmountInBaseAsync(assetIdFrom, 1, assetIdTo);
            }
            catch (Exception exception)
            {
                _log.WarningWithDetails("Could not get asset rate.", exception, new
                {
                    AssetIdFrom = assetIdFrom,
                    AssetIdTo = assetIdTo
                });
            }

            return rate;
        }

        private static string GetCacheKey(string assetId)
        {
            return $"Rate_{assetId}";
        }
    }
}
