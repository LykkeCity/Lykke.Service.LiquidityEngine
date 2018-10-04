using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class AssetPairLinksController : Controller, IAssetPairLinksApi
    {
        private readonly IAssetPairLinkService _assetPairLinkService;

        public AssetPairLinksController(IAssetPairLinkService assetPairLinkService)
        {
            _assetPairLinkService = assetPairLinkService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair links.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AssetPairLinkModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<AssetPairLinkModel>> GetAsync()
        {
            IReadOnlyCollection<AssetPairLink> assetPairLinks = await _assetPairLinkService.GetAllAsync();

            return Mapper.Map<List<AssetPairLinkModel>>(assetPairLinks);
        }

        /// <inheritdoc/>
        /// <response code="204">The asset pair link successfully added.</response>
        /// <response code="409">Asset pair link already exists.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.Conflict)]
        public async Task AddAsync([FromBody] AssetPairLinkModel model)
        {
            try
            {
                AssetPairLink assetPairLink = Mapper.Map<AssetPairLink>(model);
                
                await _assetPairLinkService.AddAsync(assetPairLink);
            }
            catch (EntityAlreadyExistsException)
            {
                throw new ValidationApiException(HttpStatusCode.Conflict, "Asset pair link already exists.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The asset pair link successfully deleted.</response>
        /// <response code="404">Asset pair link does not exist.</response>
        [HttpDelete("{assetPairId}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string assetPairId)
        {
            try
            {
                await _assetPairLinkService.DeleteAsync(assetPairId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Asset pair link does not exist.");
            }
        }
    }
}
