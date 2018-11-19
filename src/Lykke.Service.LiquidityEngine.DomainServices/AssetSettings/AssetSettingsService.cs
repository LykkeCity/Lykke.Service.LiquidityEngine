using System;
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
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Domain.Consts;

namespace Lykke.Service.LiquidityEngine.DomainServices.AssetSettings
{
    [UsedImplicitly]
    public class AssetSettingsService : IAssetSettingsService
    {
        /// <summary>
        /// Explicitly specified constant settings for USD.
        /// </summary>
        private static readonly Domain.AssetSettings UsdAssetSettings = new Domain.AssetSettings
        {
            AssetId = AssetConsts.UsdAssetId,
            LykkeAssetId = AssetConsts.UsdAssetId,
            ExternalAssetPairId = "",
            QuoteSource = "",
            IsInverse = false,
            IsCrypto = false,
            DisplayAccuracy = 2
        };

        private readonly IAssetSettingsRepository _assetSettingsRepository;
        private readonly IQuoteService _quoteService;
        private readonly InMemoryCache<Domain.AssetSettings> _cache;
        private readonly ILog _log;

        public AssetSettingsService(
            IAssetSettingsRepository assetSettingsRepository,
            IQuoteService quoteService,
            ILogFactory logFactory)
        {
            _assetSettingsRepository = assetSettingsRepository;
            _quoteService = quoteService;
            _cache = new InMemoryCache<Domain.AssetSettings>(asset => asset.AssetId, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<Domain.AssetSettings>> GetAllAsync()
        {
            IReadOnlyCollection<Domain.AssetSettings> assets = _cache.GetAll();

            if (assets == null)
            {
                assets = (await _assetSettingsRepository.GetAllAsync())
                    .Append(UsdAssetSettings)
                    .ToArray();

                _cache.Initialize(assets);
            }

            return assets;
        }

        public async Task<Domain.AssetSettings> GetByIdAsync(string assetId)
        {
            IReadOnlyCollection<Domain.AssetSettings> assets = await GetAllAsync();

            Domain.AssetSettings assetSettings = assets.SingleOrDefault(o => o.AssetId == assetId);

            if (assetSettings == null)
                throw new EntityNotFoundException();

            return assetSettings;
        }

        public async Task AddAsync(Domain.AssetSettings assetSettings)
        {
            Domain.AssetSettings existingAssetSettings = (await GetAllAsync())
                .SingleOrDefault(o => o.AssetId == assetSettings.AssetId);

            if (existingAssetSettings != null)
                throw new EntityAlreadyExistsException();

            await _assetSettingsRepository.InsertAsync(assetSettings);

            _cache.Set(assetSettings);

            _log.InfoWithDetails("Asset settings were added", assetSettings);
        }

        public async Task UpdateAsync(Domain.AssetSettings assetSettings)
        {
            if (assetSettings.AssetId == AssetConsts.UsdAssetId)
            {
                throw new InvalidOperationException("Can not update non-editable asset.");
            }

            Domain.AssetSettings currentAssetSettings = await GetByIdAsync(assetSettings.AssetId);

            currentAssetSettings.Update(assetSettings);

            await _assetSettingsRepository.UpdateAsync(currentAssetSettings);

            _cache.Set(currentAssetSettings);

            _log.InfoWithDetails("Asset settings were updated", assetSettings);
        }

        public async Task DeleteAsync(string assetId)
        {
            if (assetId == AssetConsts.UsdAssetId)
            {
                throw new InvalidOperationException("Can not delete non-editable asset.");
            }
            
            Domain.AssetSettings assetSettings = await GetByIdAsync(assetId);

            await _assetSettingsRepository.DeleteAsync(assetId);

            _cache.Remove(assetId);

            _log.InfoWithDetails("Asset settings were deleted", assetSettings);
        }

        public async Task<(decimal?, decimal?)> ConvertAmountAsync(string assetId, decimal amount)
        {
            if (assetId == AssetConsts.UsdAssetId)
                return (amount, 1);
            
            IReadOnlyCollection<Domain.AssetSettings> assetSettings = await GetAllAsync();

            Domain.AssetSettings settings = assetSettings.SingleOrDefault(o => o.AssetId == assetId);

            if (settings == null)
            {
                _log.WarningWithDetails("Asset settings not found", assetId);
                return (null, null);
            }

            Quote quote = await _quoteService
                .GetAsync(settings.QuoteSource, settings.ExternalAssetPairId);

            if (quote == null)
            {
                _log.WarningWithDetails("No quote for asset", settings);
                return (null, null);
            }

            (decimal amountInUsd, decimal rate) = Calculator.CalculateCrossMidPrice(amount, quote, settings.IsInverse);

            return (amountInUsd, rate);
        }
    }
}
