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

namespace Lykke.Service.LiquidityEngine.DomainServices.Instruments
{
    public class InstrumentService : IInstrumentService
    {
        private readonly IInstrumentRepository _instrumentRepository;
        private readonly ICrossInstrumentRepository _crossInstrumentRepository;
        private readonly IRemainingVolumeService _remainingVolumeService;
        private readonly InMemoryCache<Instrument> _cache;
        private readonly ILog _log;

        public InstrumentService(
            IInstrumentRepository instrumentRepository,
            ICrossInstrumentRepository crossInstrumentRepository,
            IRemainingVolumeService remainingVolumeService,
            ILogFactory logFactory)
        {
            _instrumentRepository = instrumentRepository;
            _crossInstrumentRepository = crossInstrumentRepository;
            _remainingVolumeService = remainingVolumeService;
            _cache = new InMemoryCache<Instrument>(instrument => instrument.AssetPairId, false);
            _log = logFactory.CreateLog(this);
        }

        public async Task<IReadOnlyCollection<Instrument>> GetAllAsync()
        {
            IReadOnlyCollection<Instrument> instruments = _cache.GetAll();

            if (instruments == null)
            {
                instruments = await _instrumentRepository.GetAllAsync();

                foreach (Instrument instrument in instruments)
                    instrument.CrossInstruments = await _crossInstrumentRepository.GetAsync(instrument.AssetPairId);

                _cache.Initialize(instruments);
            }

            return instruments;
        }

        public async Task<Instrument> GetByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<Instrument> instruments = await GetAllAsync();

            Instrument instrument = instruments.FirstOrDefault(o => o.AssetPairId == assetPairId);

            if (instrument == null)
                throw new EntityNotFoundException();

            return instrument;
        }

        public async Task AddAsync(Instrument instrument)
        {
            IReadOnlyCollection<Instrument> instruments = await GetAllAsync();

            if (instruments.Any(o => o.AssetPairId == instrument.AssetPairId ||
                                     o.CrossInstruments?.Any(p => p.AssetPairId == instrument.AssetPairId) ==
                                     true))
            {
                throw new InvalidOperationException("The instrument already used");
            }

            await _instrumentRepository.InsertAsync(instrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Instrument was added", instrument);
        }

        public async Task UpdateAsync(Instrument instrument)
        {
            Instrument currentInstrument = await GetByAssetPairIdAsync(instrument.AssetPairId);

            currentInstrument.Update(instrument);

            await _instrumentRepository.UpdateAsync(currentInstrument);

            _cache.Set(currentInstrument);

            _log.InfoWithDetails("Instrument was updated", currentInstrument);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            if (instrument.Mode == InstrumentMode.Active)
                throw new InvalidOperationException("Can not remove active instrument");

            IReadOnlyCollection<RemainingVolume> remainingVolumes = await _remainingVolumeService.GetAllAsync();

            RemainingVolume remainingVolume =
                remainingVolumes.SingleOrDefault(o => o.AssetPairId == instrument.AssetPairId);

            if (remainingVolume != null && remainingVolume.Volume != 0)
                throw new InvalidOperationException("Can not remove instrument while remaining volume exist");

            await _instrumentRepository.DeleteAsync(assetPairId);

            await _crossInstrumentRepository.DeleteAsync(assetPairId);

            await _remainingVolumeService.DeleteAsync(assetPairId);

            _cache.Remove(assetPairId);

            _log.InfoWithDetails("Instrument was deleted", instrument);
        }

        public async Task AddLevelAsync(string assetPairId, InstrumentLevel instrumentLevel)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.AddLevel(instrumentLevel);

            await _instrumentRepository.UpdateAsync(instrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Level volume was added to the instrument", instrument);
        }

        public async Task UpdateLevelAsync(string assetPairId, InstrumentLevel instrumentLevel)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.UpdateLevel(instrumentLevel);

            await _instrumentRepository.UpdateAsync(instrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Level volume was updated of the instrument", instrument);
        }

        public async Task RemoveLevelAsync(string assetPairId, string levelId)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.RemoveLevel(levelId);

            await _instrumentRepository.UpdateAsync(instrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Level volume was removed from the instrument", instrument);
        }

        public async Task RemoveLevelByNumberAsync(string assetPairId, int levelNumber)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.RemoveLevelByNumber(levelNumber);

            await _instrumentRepository.UpdateAsync(instrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Level volume was removed from the instrument", instrument);
        }

        public async Task AddCrossInstrumentAsync(string assetPairId, CrossInstrument crossInstrument)
        {
            IReadOnlyCollection<Instrument> instruments = await GetAllAsync();

            if (instruments.Any(o => o.AssetPairId == crossInstrument.AssetPairId ||
                                     o.CrossInstruments?.Any(p => p.AssetPairId == crossInstrument.AssetPairId) ==
                                     true))
            {
                throw new InvalidOperationException("The instrument already used");
            }

            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.AddCrossInstrument(crossInstrument);

            await _crossInstrumentRepository.AddAsync(assetPairId, crossInstrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Cross instrument was added to the instrument", instrument);
        }

        public async Task UpdateCrossInstrumentAsync(string assetPairId, CrossInstrument crossInstrument)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.UpdateCrossInstrument(crossInstrument);

            await _crossInstrumentRepository.UpdateAsync(assetPairId, crossInstrument);

            _cache.Set(instrument);

            _log.InfoWithDetails("Cross instrument was updated of the instrument", instrument);
        }

        public async Task RemoveCrossInstrumentAsync(string assetPairId, string crossAssetPairId)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);

            instrument.RemoveCrossInstrument(crossAssetPairId);

            await _crossInstrumentRepository.DeleteAsync(assetPairId, crossAssetPairId);

            _cache.Set(instrument);

            _log.InfoWithDetails("Cross instrument was removed from the instrument", instrument);
        }
    }
}
