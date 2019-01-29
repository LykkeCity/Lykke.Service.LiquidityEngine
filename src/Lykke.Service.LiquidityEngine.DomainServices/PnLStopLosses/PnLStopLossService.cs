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

        public Task<IReadOnlyCollection<PnLStopLossSettings>> GetAllSettingsAsync()
        {
            IReadOnlyCollection<PnLStopLossSettings> pnLStopLossSettings = _settingsCache.GetAll();

            return Task.FromResult(pnLStopLossSettings);
        }

        public async Task AddSettingsAsync(PnLStopLossSettings pnLStopLossSettings)
        {
            if (!string.IsNullOrWhiteSpace(pnLStopLossSettings.Id))
                throw new InvalidOperationException("PnL stop loss settings already has an identifier.");

            if (_instrumentService.TryGetByAssetPairIdAsync(pnLStopLossSettings.AssetPairId) == null)
                throw new InvalidOperationException($"Asset pair id is not found: '{pnLStopLossSettings.AssetPairId}'.");

            string assetPairId = pnLStopLossSettings.AssetPairId;

            // if settings are local for particular instrument then just create an engine
            if (!string.IsNullOrWhiteSpace(assetPairId))
            {
                _log.InfoWithDetails("Creating pnl stop loss engine.", pnLStopLossSettings);

                PnLStopLossEngine newEngine = new PnLStopLossEngine(pnLStopLossSettings);

                await CreateEngine(newEngine);

                _log.InfoWithDetails("PnL stop loss engine created.", pnLStopLossSettings);
            }
            // if settings are global then create global settings in the DB and an engine for each instrument
            else
            {
                _log.InfoWithDetails("Creating pnl stop loss engines.", pnLStopLossSettings);

                pnLStopLossSettings.Id = Guid.NewGuid().ToString();

                await _pnLStopLossSettingsRepository.InsertAsync(pnLStopLossSettings);

                _settingsCache.Set(pnLStopLossSettings);

                _log.InfoWithDetails("PnL stop loss settings created.", pnLStopLossSettings);

                IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

                foreach (var instrument in instruments)
                {
                    await CreateEngine(instrument.AssetPairId, pnLStopLossSettings);
                }

                _log.InfoWithDetails("PnL stop loss engines created.", pnLStopLossSettings);
            }
        }

        public async Task RefreshSettingsAsync(string id)
        {
            _log.InfoWithDetails("Refreshing pnl stop loss settings.", id);

            PnLStopLossSettings settings = await GetSettingsByIdAsync(id);

            IReadOnlyCollection<PnLStopLossEngine> existedEngines = await GetAllEnginesAsync();

            IReadOnlyCollection<string> existedAssetPairs =
                existedEngines.Where(x => x.PnLStopLossSettingsId == id)
                              .Select(x => x.AssetPairId)
                              .ToList();

            IReadOnlyCollection<string> allAssetPairs = (await _instrumentService.GetAllAsync())
                .Select(x => x.AssetPairId).ToList();

            IReadOnlyCollection<string> missedAssetPairIds = allAssetPairs.Except(existedAssetPairs).ToList();

            foreach (var assetPairId in missedAssetPairIds)
            {
                await CreateEngine(assetPairId, settings);
            }

            _log.InfoWithDetails("PnL stop loss settings refreshed.", id);
        }

        public async Task DeleteSettingsAsync(string id)
        {
            await GetSettingsByIdAsync(id);

            await _pnLStopLossSettingsRepository.DeleteAsync(id);

            _settingsCache.Remove(id);

            _log.InfoWithDetails("PnL stop loss settings deleted.", id);
        }


        public Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllEnginesAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = _enginesCache.GetAll();

            return Task.FromResult(pnLStopLossEngines);
        }

        public async Task UpdateEngineAsync(PnLStopLossEngine pnLStopLossEngine)
        {
            PnLStopLossEngine currentPnLStopLossEngine = await GetEngineByIdAsync(pnLStopLossEngine.Id);

            currentPnLStopLossEngine.Update(pnLStopLossEngine);

            await _pnLStopLossEngineRepository.UpdateAsync(currentPnLStopLossEngine);

            _enginesCache.Set(currentPnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine updated.", currentPnLStopLossEngine);
        }

        public async Task DeleteEngineAsync(string id)
        {
            await GetEngineByIdAsync(id);

            await _pnLStopLossEngineRepository.DeleteAsync(id);

            _enginesCache.Remove(id);

            _log.InfoWithDetails("PnL stop loss engine deleted.", id);
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

            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = await GetEnginesByAssetPairIdAsync(position.AssetPairId);

            if (!pnLStopLossEngines.Any())
                return;

            decimal? pnlUsd = await _crossRateInstrumentService.ConvertPriceAsync(position.AssetPairId,
                position.PnL);

            if (!pnlUsd.HasValue)
            {
                _log.Warning($"Can't convert quote asset to USD for '{position.AssetPairId}'. Skipped pnl stop loss calculation.");

                return;
            }

            foreach (PnLStopLossEngine pnLStopLossEngine in pnLStopLossEngines)
            {
                pnLStopLossEngine.AddPnL(pnlUsd.Value);

                await UpdateEngine(pnLStopLossEngine);
            }

            if (pnLStopLossEngines.Any())
                _log.InfoWithDetails("Applied position to pnl stop loss engines: " +
                                     $"{string.Join(", ", pnLStopLossEngines.Select(x => x.AssetPairId).ToList())}.", position);
        }

        public async Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<PnLStopLossEngine> engines = await GetAllEnginesAsync();

            engines = engines.Where(x => x.AssetPairId == assetPairId
                                         && x.Mode == PnLStopLossEngineMode.Active
                                         && x.TotalNegativePnL <= x.Threshold).ToList();

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

        private async Task CreateEngine(string assetPairId, PnLStopLossSettings pnLStopLossSettings)
        {
            var currentPnLStopLossSettings = new PnLStopLossSettings(pnLStopLossSettings)
            {
                AssetPairId = assetPairId
            };

            PnLStopLossEngine newEngine = new PnLStopLossEngine(currentPnLStopLossSettings);

            await CreateEngine(newEngine);
        }

        private async Task UpdateEngine(PnLStopLossEngine pnLStopLossEngine)
        {
            await _pnLStopLossEngineRepository.UpdateAsync(pnLStopLossEngine);

            _enginesCache.Set(pnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine updated.", pnLStopLossEngine);
        }

        private async Task<PnLStopLossSettings> GetSettingsByIdAsync(string id)
        {
            IReadOnlyCollection<PnLStopLossSettings> pnLStopLossSettings = await GetAllSettingsAsync();

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
