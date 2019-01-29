using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.PnLStopLosses
{
    public class PnLStopLossService : IPnLStopLossService
    {
        private readonly IPnLStopLossSettingsRepository _pnLStopLossSettingsRepository;
        private readonly InMemoryCache<PnLStopLossSettings> _settingsCache;
        private readonly IPnLStopLossEngineRepository _pnLStopLossEngineRepository;
        private readonly InMemoryCache<PnLStopLossEngine> _enginesCache;
        private readonly IInstrumentService _instrumentService;
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;
        private readonly ILog _log;

        public PnLStopLossService(
            IPnLStopLossSettingsRepository pnLStopLossSettingsRepository,
            IPnLStopLossEngineRepository pnLStopLossEngineRepository,
            IInstrumentService instrumentService,
            ICrossRateInstrumentService crossRateInstrumentService,
            ILogFactory logFactory)
        {
            _pnLStopLossSettingsRepository = pnLStopLossSettingsRepository;
            _settingsCache = new InMemoryCache<PnLStopLossSettings>(settings => settings.Id, false);
            _pnLStopLossEngineRepository = pnLStopLossEngineRepository;
            _enginesCache = new InMemoryCache<PnLStopLossEngine>(engine => engine.Id, false);
            _instrumentService = instrumentService;
            _crossRateInstrumentService = crossRateInstrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task Initialize()
        {
            // _settingsCache

            var pnLStopLossSettings = await _pnLStopLossSettingsRepository.GetAllAsync();

            _settingsCache.Initialize(pnLStopLossSettings);

            // _enginesCache

            var pnLStopLossEngines = await _pnLStopLossEngineRepository.GetAllAsync();

            _enginesCache.Initialize(pnLStopLossEngines);
        }

        public async Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllEnginesAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = _enginesCache.GetAll();

            return pnLStopLossEngines;
        }

        public async Task CreateAsync(PnLStopLossSettings pnLStopLossSettings)
        {
            if (!string.IsNullOrWhiteSpace(pnLStopLossSettings.Id))
                throw new InvalidOperationException("The pnl stop loss already has an identifier.");

            if (_instrumentService.TryGetByAssetPairIdAsync(pnLStopLossSettings.AssetPairId) == null)
                throw new InvalidOperationException($"Asset pair id is not found: '{pnLStopLossSettings.AssetPairId}'.");

            string assetPairId = pnLStopLossSettings.AssetPairId;

            // if settings are local for particular instrument then just create it
            if (!string.IsNullOrWhiteSpace(assetPairId))
            {
                _log.InfoWithDetails("Creating new pnl stop loss engine from local settings.", pnLStopLossSettings);

                PnLStopLossEngine newEngine = PnLStopLossEngine.CreateFromLocalSettings(pnLStopLossSettings);

                await CreateEngine(newEngine);
            }
            // if settings are global then create global settings in the DB and then create engines for each instrument
            else
            {
                _log.InfoWithDetails("Creating new pnl stop loss engines from global settings.", pnLStopLossSettings);

                pnLStopLossSettings.Id = Guid.NewGuid().ToString();

                await _pnLStopLossSettingsRepository.InsertAsync(pnLStopLossSettings);

                _settingsCache.Set(pnLStopLossSettings);

                _log.InfoWithDetails("PnL stop loss global settings created.", pnLStopLossSettings);

                IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

                foreach (var instrument in instruments)
                {
                    PnLStopLossEngine newEngine = PnLStopLossEngine.CreateFromGlobalSettings(instrument.AssetPairId, pnLStopLossSettings);

                    await CreateEngine(newEngine);
                }
            }
        }

        public async Task DeleteEngineAsync(string id)
        {
            await GetEngineByIdAsync(id);

            await _pnLStopLossEngineRepository.DeleteAsync(id);

            _enginesCache.Remove(id);

            _log.InfoWithDetails("PnL stop loss engine deleted.", id);
        }

        public async Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllGlobalSettingsAsync()
        {
            IReadOnlyCollection<PnLStopLossSettings> pnLStopLossSettings = _settingsCache.GetAll();

            return pnLStopLossSettings;
        }

        public async Task ReapplyGlobalSettingsAsync(string id)
        {
            _log.InfoWithDetails("Start reapplying pnl stop loss global settings.", id);

            PnLStopLossSettings globalSettings = await GetGlobalSettingsByIdAsync(id);

            IReadOnlyCollection<PnLStopLossEngine> existedEngines = await GetAllEnginesAsync();

            IReadOnlyCollection<string> existedAssetPairs =
                existedEngines.Where(x => x.PnLStopLossGlobalSettingsId == id)
                              .Select(x => x.AssetPairId)
                              .ToList();

            IReadOnlyCollection<string> allAssetPairs = (await _instrumentService.GetAllAsync())
                .Select(x => x.AssetPairId).ToList();

            IReadOnlyCollection<string> missedAssetPairIds = allAssetPairs.Except(existedAssetPairs).ToList();

            foreach (var assetPairId in missedAssetPairIds)
            {
                PnLStopLossEngine newEngine = PnLStopLossEngine.CreateFromGlobalSettings(assetPairId, globalSettings);

                await CreateEngine(newEngine);
            }

            _log.InfoWithDetails("PnL stop loss global settings reapplied.", id);
        }

        public async Task DeleteGlobalSettingsAsync(string id)
        {
            await GetGlobalSettingsByIdAsync(id);

            await _pnLStopLossSettingsRepository.DeleteAsync(id);

            _settingsCache.Remove(id);

            _log.InfoWithDetails("PnL stop loss global settings deleted.", id);
        }

        public async Task UpdateEngineModeAsync(string id, PnLStopLossEngineMode mode)
        {
            PnLStopLossEngine pnLStopLossEngine = await GetEngineByIdAsync(id);

            if (pnLStopLossEngine.Mode == mode)
                throw new InvalidOperationException($"Engine mode does not differ from existing: '{mode}'.");

            pnLStopLossEngine.ChangeMode(mode);

            _pnLStopLossEngineRepository.UpdateAsync(pnLStopLossEngine).GetAwaiter().GetResult();

            _enginesCache.Set(pnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine updated.", pnLStopLossEngine);
        }


        public async Task ExecuteAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> allPnLStopLossEngines = await GetAllEnginesAsync();

            foreach (var pnLStopLossEngine in allPnLStopLossEngines)
                await RefreshEngine(pnLStopLossEngine);
        }

        public async Task HandleClosedPositionAsync(Position position)
        {
            if (position.PnL >= 0)
                return;

            _log.InfoWithDetails("Received position with negative PnL.", position);

            decimal? pnlUsd = await _crossRateInstrumentService.ConvertPriceAsync(position.AssetPairId,
                position.PnL);

            if (!pnlUsd.HasValue)
            {
                _log.Warning($"Can't convert quote asset to USD for '{position.AssetPairId}'. Skipped pnl stop loss calculation.");

                return;
            }

            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = await GetEnginesByAssetPairIdAsync(position.AssetPairId);

            foreach (PnLStopLossEngine pnLStopLossEngine in pnLStopLossEngines)
            {
                pnLStopLossEngine.AddPnL(pnlUsd.Value);

                await UpdateEngine(pnLStopLossEngine);
            }
        }

        public async Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<PnLStopLossEngine> engines = await GetAllEnginesAsync();

            engines = engines.Where(x => x.AssetPairId == assetPairId
                                         && x.Mode == PnLStopLossEngineMode.Active
                                         && x.TotalNegativePnL <= x.PnLThreshold).ToList();

            decimal totalMarkup = engines.Sum(x => x.Markup);

            return totalMarkup;
        }


        private async Task RefreshEngine(PnLStopLossEngine pnLStopLossEngine)
        {
            pnLStopLossEngine.Refresh();

            await UpdateEngine(pnLStopLossEngine);
        }

        private async Task CreateEngine(PnLStopLossEngine pnLStopLossEngine)
        {
            await _pnLStopLossEngineRepository.InsertAsync(pnLStopLossEngine);

            _enginesCache.Set(pnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine created.", pnLStopLossEngine);
        }

        private async Task UpdateEngine(PnLStopLossEngine pnLStopLossEngine)
        {
            await _pnLStopLossEngineRepository.UpdateAsync(pnLStopLossEngine);

            _enginesCache.Set(pnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine updated.", pnLStopLossEngine);
        }

        private async Task<PnLStopLossSettings> GetGlobalSettingsByIdAsync(string id)
        {
            IReadOnlyCollection<PnLStopLossSettings> pnLStopLossSettings = await GetAllGlobalSettingsAsync();

            PnLStopLossSettings result = pnLStopLossSettings.FirstOrDefault(o => o.Id == id);

            if (pnLStopLossSettings == null)
                throw new EntityNotFoundException();

            return result;
        }

        private async Task<PnLStopLossEngine> GetEngineByIdAsync(string id)
        {
            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = await GetAllEnginesAsync();

            PnLStopLossEngine pnLStopLossSettings = pnLStopLossEngines.FirstOrDefault(o => o.Id == id);

            if (pnLStopLossSettings == null)
                throw new EntityNotFoundException();

            return pnLStopLossSettings;
        }

        private async Task<IReadOnlyCollection<PnLStopLossEngine>> GetEnginesByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = await GetAllEnginesAsync();

            pnLStopLossEngines = pnLStopLossEngines.Where(x => x.AssetPairId == assetPairId).ToList();

            return pnLStopLossEngines;
        }
    }
}
