using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Log;
using JetBrains.Annotations;
using Lykke.Common.Log;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Extensions;
using Lykke.Service.LiquidityEngine.Domain.Repositories;
using Lykke.Service.LiquidityEngine.Domain.Services;

namespace Lykke.Service.LiquidityEngine.DomainServices.AssetPairLinks
{
    [UsedImplicitly]
    public class AssetPairLinkService : IAssetPairLinkService
    {
        private readonly IAssetPairLinkRepository _assetPairLinkRepository;
        private readonly InMemoryCache<AssetPairLink> _cache;
        private readonly ILog _log;

        public AssetPairLinkService(IAssetPairLinkRepository assetPairLinkRepository, ILogFactory logFactory)
        {
            _assetPairLinkRepository = assetPairLinkRepository;
            _cache = new InMemoryCache<AssetPairLink>(assetPairLink => assetPairLink.AssetPairId);
            _log = logFactory.CreateLog(this);
        }
        
        public async Task<IReadOnlyCollection<AssetPairLink>> GetAllAsync()
        {
            IReadOnlyCollection<AssetPairLink> assetPairLinks = _cache.GetAll();

            if (assetPairLinks == null)
            {
                assetPairLinks = await _assetPairLinkRepository.GetAllAsync();
                _cache.Initialize(assetPairLinks);
            }

            return assetPairLinks;
        }

        public async Task AddAsync(AssetPairLink assetPairLink)
        {
            await _assetPairLinkRepository.AddAsync(assetPairLink);
            
            _cache.Set(assetPairLink);
            
            _log.InfoWithDetails("Asset pair link was added", assetPairLink);
        }

        public async Task DeleteAsync(string assetPairId)
        {
            IReadOnlyCollection<AssetPairLink> assetPairLinks = await GetAllAsync();

            AssetPairLink assetPairLink = assetPairLinks.FirstOrDefault(o => o.AssetPairId == assetPairId);
    
            if (assetPairLink == null)
                throw new EntityNotFoundException();
            
            await _assetPairLinkRepository.DeleteAsync(assetPairId);
            
            _cache.Remove(assetPairId);
            
            _log.InfoWithDetails("Asset pair link was removed", assetPairLink);
        }
    }
}
