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
        private readonly InMemoryCache<Instrument> _cache;
        private readonly ILog _log;

        public InstrumentService(IInstrumentRepository instrumentRepository, ILogFactory logFactory)
        {
            _instrumentRepository = instrumentRepository;
            _cache = new InMemoryCache<Instrument>(instrument => instrument.AssetPairId, false);
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<IReadOnlyCollection<Instrument>> GetAllAsync()
        {
            IReadOnlyCollection<Instrument> instruments = _cache.GetAll();

            if (instruments == null)
            {
                instruments = await _instrumentRepository.GetAllAsync();
                
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
            
            await _instrumentRepository.DeleteAsync(assetPairId);
            
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

        public async Task RemoveLevelAsync(string assetPairId, int levelNumber)
        {
            Instrument instrument = await GetByAssetPairIdAsync(assetPairId);
            
            instrument.RemoveLevel(levelNumber);
            
            await _instrumentRepository.UpdateAsync(instrument);
            
            _cache.Set(instrument);
            
            _log.InfoWithDetails("Level volume was removed from the instrument", instrument);
        }
    }
}
