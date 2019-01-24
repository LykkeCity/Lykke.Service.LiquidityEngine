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
        private readonly ILog _log;

        public PnLStopLossService(
            IPnLStopLossSettingsRepository pnLStopLossSettingsRepository,
            IPnLStopLossEngineRepository pnLStopLossEngineRepository,
            IInstrumentService instrumentService,
            ILogFactory logFactory)
        {
            _pnLStopLossSettingsRepository = pnLStopLossSettingsRepository;
            _settingsCache = new InMemoryCache<PnLStopLossSettings>(settings => settings.Id, false);
            _pnLStopLossEngineRepository = pnLStopLossEngineRepository;
            _enginesCache = new InMemoryCache<PnLStopLossEngine>(engine => engine.Id, false);
            _instrumentService = instrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<PnLStopLossEngine>> GetAllEnginesAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> pnLStopLossEngines = _enginesCache.GetAll();

            if (pnLStopLossEngines == null)
            {
                pnLStopLossEngines = await _pnLStopLossEngineRepository.GetAllAsync();

                _enginesCache.Initialize(pnLStopLossEngines);
            }

            return pnLStopLossEngines;
        }

        public async Task CreateAsync(PnLStopLossSettings pnLStopLossSettings)
        {
            if (!string.IsNullOrWhiteSpace(pnLStopLossSettings.Id))
                throw new InvalidOperationException("The pnl stop loss already has an identifier.");

            if (_instrumentService.TryGetByAssetPairIdAsync(pnLStopLossSettings.AssetPairId) == null)
                throw new InvalidOperationException($"Asset pair id is not found: '{pnLStopLossSettings.AssetPairId}'.");

            // 1. If local for particular instrument then just create it

            // 2. If global then create an engine for each instrument and add global settings to the DB

            //IReadOnlyCollection<PnLStopLossSettings> pnLStopLosses = await GetAllAsync();

            //pnLStopLossSettings.Id = Guid.NewGuid().ToString();

            //await _pnLStopLossSettingsRepository.InsertAsync(pnLStopLossSettings);

            //_cache.Set(pnLStopLosses);

            //_log.InfoWithDetails("PnL stop loss created.", pnLStopLossSettings);

            throw new NotImplementedException();

            _log.InfoWithDetails("PnL stop loss engines created.", pnLStopLossSettings);
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

            if (pnLStopLossSettings == null)
            {
                pnLStopLossSettings = await _pnLStopLossSettingsRepository.GetAllAsync();

                _settingsCache.Initialize(pnLStopLossSettings);
            }

            return pnLStopLossSettings;
        }

        public async Task ReapplyGlobalSettingsAsync(string id)
        {
            // Find all asset pairs without passed settings and add it.

            throw new NotImplementedException();

            _log.InfoWithDetails("PnL stop loss global settings reapplied.", id);
        }

        public async Task DeleteGlobalSettingsAsync(string id)
        {
            await GetSettingsByIdAsync(id);

            await _pnLStopLossSettingsRepository.DeleteAsync(id);

            _settingsCache.Remove(id);

            _log.InfoWithDetails("PnL stop loss global settings deleted.", id);
        }

        public async Task UpdateEngineModeAsync(string id, PnLStopLossEngineMode mode)
        {
            PnLStopLossEngine pnLStopLossEngine = await GetEngineByIdAsync(id);

            if (pnLStopLossEngine.Mode == mode)
                throw new InvalidOperationException($"Engine mode does not differ from existing: '{mode}'.");

            pnLStopLossEngine.Mode = mode;

            await _pnLStopLossEngineRepository.UpdateAsync(pnLStopLossEngine);

            _enginesCache.Set(pnLStopLossEngine);

            _log.InfoWithDetails("PnL stop loss engine updated.", pnLStopLossEngine);
        }

        public async Task HandleClosedPositionAsync(Position position)
        {
            // TODO: !!! May executes from many threads, must be lock'ed?

            if (position.PnL >= 0)
                return;

            var pnLStopLossEngines = await GetEnginesByAssetPairIdAsync(position.AssetPairId);

            //foreach (var engine in pnLStopLossEngines)
            //    await ApplyNewPositionToEngine(engine, position);
        }

        //private Task ApplyNewPositionToEngine(PnLStopLossEngine pnLStopLossEngine, Position position)
        //{

        //}

        public async Task<decimal> GetTotalMarkupByAssetPairIdAsync(string assetPairId)
        {
            var engines = await GetAllEnginesAsync();

            engines = engines.Where(x => x.Mode == PnLStopLossEngineMode.Active
                                      && x.AssetPairId == assetPairId).ToList();

            var totalMarkup = engines.Sum(x => x.Markup);

            return totalMarkup;
        }

        private async Task<PnLStopLossSettings> GetSettingsByIdAsync(string id)
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
