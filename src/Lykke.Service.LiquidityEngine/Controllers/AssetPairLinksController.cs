using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class AssetPairLinksController : Controller, IAssetPairLinksApi
    {
        public AssetPairLinksController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair links.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AssetPairLinkModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<AssetPairLinkModel>> GetAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The asset pair link successfully added.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task AddAsync([FromBody] AssetPairLinkModel model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The asset pair link successfully removed.</response>
        [HttpDelete("{assetPairId}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task RemoveAsync(string assetPairId)
        {
            throw new System.NotImplementedException();
        }
    }
}
