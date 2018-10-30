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

namespace Lykke.Service.LiquidityEngine.DomainServices.CrossRateInstruments
{
    public class CrossRateInstrumentService : ICrossRateInstrumentService
    {
        private readonly ICrossRateInstrumentRepository _crossInstrumentRepository;
        private readonly InMemoryCache<CrossRateInstrument> _cache;
        private readonly ILog _log;

        public CrossRateInstrumentService(ICrossRateInstrumentRepository crossInstrumentRepository, ILogFactory logFactory)
        {
            _crossInstrumentRepository = crossInstrumentRepository;
            _cache = new InMemoryCache<CrossRateInstrument>(instrument => instrument.AssetPairId, false);
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<IReadOnlyCollection<CrossRateInstrument>> GetAllAsync()
        {
            IReadOnlyCollection<CrossRateInstrument> crossInstruments = _cache.GetAll();

            if (crossInstruments == null)
            {
                crossInstruments = await _crossInstrumentRepository.GetAllAsync();
                
                _cache.Initialize(crossInstruments);
            }

            return crossInstruments;
        }

        public async Task<CrossRateInstrument> GetByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<CrossRateInstrument> crossInstruments = await GetAllAsync();

            CrossRateInstrument crossInstrument = crossInstruments.FirstOrDefault(o => o.AssetPairId == assetPairId);

            if (crossInstrument == null)
                throw new EntityNotFoundException();

            return crossInstrument;
        }

        public async Task AddAsync(CrossRateInstrument crossInstrument)
        {
            await _crossInstrumentRepository.InsertAsync(crossInstrument);
            
            _cache.Set(crossInstrument);

            _log.InfoWithDetails("Cross instrument was added", crossInstrument);
        }

        public async Task UpdateAsync(CrossRateInstrument crossInstrument)
        {
            CrossRateInstrument currentCrossInstrument = await GetByAssetPairIdAsync(crossInstrument.AssetPairId);
            
            currentCrossInstrument.Update(crossInstrument);
            
            await _crossInstrumentRepository.UpdateAsync(currentCrossInstrument);
            
            _cache.Set(currentCrossInstrument);
            
            _log.InfoWithDetails("Cross instrument was updated", currentCrossInstrument);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            CrossRateInstrument crossInstrument = await GetByAssetPairIdAsync(assetPairId);

            await _crossInstrumentRepository.DeleteAsync(assetPairId);

            _cache.Remove(assetPairId);

            _log.InfoWithDetails("Cross instrument was deleted", crossInstrument);
        }
    }
}
