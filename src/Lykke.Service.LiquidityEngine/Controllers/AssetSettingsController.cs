using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.AssetSettings;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class AssetSettingsController : Controller, IAssetSettingsApi
    {
        private readonly IAssetSettingsService _assetSettingsService;

        public AssetSettingsController(IAssetSettingsService assetSettingsService)
        {
            _assetSettingsService = assetSettingsService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset settings.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<AssetSettingsModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<AssetSettingsModel>> GetAllAsync()
        {
            IReadOnlyCollection<AssetSettings> assets = await _assetSettingsService.GetAllAsync();

            return Mapper.Map<List<AssetSettingsModel>>(assets);
        }

        /// <inheritdoc/>
        /// <response code="200">Asset settings.</response>
        /// <response code="404">Asset settings do not exist.</response>
        [HttpGet("{assetId}")]
        [ProducesResponseType(typeof(AssetSettingsModel), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
        public async Task<AssetSettingsModel> GetByIdAsync(string assetId)
        {
            AssetSettings assetSettings = await _assetSettingsService.GetByIdAsync(assetId);

            if (assetSettings == null)
                throw new ValidationApiException(HttpStatusCode.NotFound, "Asset settings do not exist.");

            return Mapper.Map<AssetSettingsModel>(assetSettings);
        }

        /// <inheritdoc/>
        /// <response code="204">The asset settings successfully added.</response>
        /// <response code="409">Asset settings already exists.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task AddAsync([FromBody] AssetSettingsModel settingsModel)
        {
            try
            {
                var asset = Mapper.Map<AssetSettings>(settingsModel);

                await _assetSettingsService.AddAsync(asset);
            }
            catch (EntityAlreadyExistsException)
            {
                throw new ValidationApiException(HttpStatusCode.Conflict, "Asset settings already exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The asset settings successfully updated.</response>
        /// <response code="400">Asset settings are non-editable.</response>
        /// <response code="404">Asset settings do not exist.</response>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.BadRequest)]
        public async Task UpdateAsync([FromBody] AssetSettingsModel settingsModel)
        {
            try
            {
                var asset = Mapper.Map<AssetSettings>(settingsModel);

                await _assetSettingsService.UpdateAsync(asset);
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Asset settings do not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The asset settings successfully deleted.</response>
        /// <response code="404">Asset settings does not exist.</response>
        [HttpDelete("{assetId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string assetId)
        {
            try
            {
                await _assetSettingsService.DeleteAsync(assetId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Asset settings do not exist.");
            }
        }
    }
}
