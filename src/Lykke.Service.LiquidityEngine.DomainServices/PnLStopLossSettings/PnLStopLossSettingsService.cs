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

namespace Lykke.Service.LiquidityEngine.DomainServices.PnLStopLossSettings
{
    public class PnLStopLossSettingsService : IPnLStopLossSettingsService
    {
        private readonly IPnLStopLossSettingsRepository _pnLStopLossSettingsRepository;
        private readonly InMemoryCache<Domain.PnLStopLossSettings> _cache;
        private readonly IPnLStopLossEngineService _pnLStopLossEngineService;
        private readonly IInstrumentService _instrumentService;
        private readonly ILog _log;

        public PnLStopLossSettingsService(
            IPnLStopLossSettingsRepository pnLStopLossSettingsRepository,
            IPnLStopLossEngineService pnLStopLossEngineService,
            IInstrumentService instrumentService,
            ILogFactory logFactory)
        {
            _pnLStopLossSettingsRepository = pnLStopLossSettingsRepository;
            _cache = new InMemoryCache<Domain.PnLStopLossSettings>(settings => settings.Id, false);
            _pnLStopLossEngineService = pnLStopLossEngineService;
            _instrumentService = instrumentService;
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<Domain.PnLStopLossSettings>> GetAllAsync()
        {
            IReadOnlyCollection<Domain.PnLStopLossSettings> pnLStopLossSettings = _cache.GetAll();

            if (pnLStopLossSettings == null)
                pnLStopLossSettings = await Initialize();

            return pnLStopLossSettings;
        }

        public async Task AddAsync(Domain.PnLStopLossSettings pnLStopLossSettings)
        {
            pnLStopLossSettings.Id = Guid.NewGuid().ToString();

            _log.InfoWithDetails("Creating pnl stop loss settings.", pnLStopLossSettings);

            await _pnLStopLossSettingsRepository.InsertAsync(pnLStopLossSettings);

            _cache.Set(pnLStopLossSettings);

            _log.InfoWithDetails("PnL stop loss settings created.", pnLStopLossSettings);

            IReadOnlyCollection<Instrument> instruments = await _instrumentService.GetAllAsync();

            foreach (var instrument in instruments)
            {
                await CreateEngine(instrument.AssetPairId, pnLStopLossSettings);
            }

            _log.InfoWithDetails("PnL stop loss settings created.", pnLStopLossSettings);
        }

        public async Task RefreshAsync(string id)
        {
            _log.InfoWithDetails("Refreshing pnl stop loss settings.", id);

            Domain.PnLStopLossSettings settings = await GetSettingsByIdAsync(id);

            IReadOnlyCollection<PnLStopLossEngine> existedEngines = await _pnLStopLossEngineService.GetAllAsync();

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

        public async Task DeleteAsync(string id)
        {
            await GetSettingsByIdAsync(id);

            await _pnLStopLossSettingsRepository.DeleteAsync(id);

            _cache.Remove(id);

            _log.InfoWithDetails("PnL stop loss settings deleted.", id);
        }

        private async Task<IReadOnlyCollection<Domain.PnLStopLossSettings>> Initialize()
        {
            IReadOnlyCollection<Domain.PnLStopLossSettings> pnLStopLossSettings = await _pnLStopLossSettingsRepository.GetAllAsync();

            _cache.Initialize(pnLStopLossSettings);

            return pnLStopLossSettings;
        }

        private async Task CreateEngine(string assetPairId, Domain.PnLStopLossSettings pnLStopLossSettings)
        {
            PnLStopLossEngine newEngine = new PnLStopLossEngine(pnLStopLossSettings);

            newEngine.AssetPairId = assetPairId;

            await _pnLStopLossEngineService.AddAsync(newEngine);
        }

        private async Task<Domain.PnLStopLossSettings> GetSettingsByIdAsync(string id)
        {
            IReadOnlyCollection<Domain.PnLStopLossSettings> pnLStopLossSettings = await GetAllAsync();

            Domain.PnLStopLossSettings result = pnLStopLossSettings.FirstOrDefault(o => o.Id == id);

            if (pnLStopLossSettings == null)
                throw new EntityNotFoundException();

            return result;
        }
    }
}
