using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLosses;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using PnLStopLossEngineMode = Lykke.Service.LiquidityEngine.Client.Models.PnLStopLosses.PnLStopLossEngineMode;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class PnLStopLossesController : Controller, IPnLStopLossesApi
    {
        private readonly IPnLStopLossService _pnLStopLossService;

        public PnLStopLossesController(IPnLStopLossService pnLStopLossService)
        {
            _pnLStopLossService = pnLStopLossService;
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss engines.</response>
        [HttpGet("/engines")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PnLStopLossEngineModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PnLStopLossEngineModel>> GetAllEnginesAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> stopLosses = await _pnLStopLossService.GetAllEnginesAsync();

            return Mapper.Map<PnLStopLossEngineModel[]>(stopLosses);
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss successfully added.</response>
        [HttpPost("/create")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task CreateAsync([FromBody] PnLStopLossSettingsModel pnLStopLossSettingsModel)
        {


            var stopLoss = Mapper.Map<PnLStopLossSettings>(pnLStopLossSettingsModel);

            await _pnLStopLossService.CreateAsync(stopLoss);
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss successfully deleted.</response>
        /// <response code="400">An error occurred while deleting pnl stop loss.</response>
        /// <response code="404">Stop loss does not exist.</response>
        [HttpDelete("/engines/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteEngineAsync(string id)
        {
            try
            {
                await _pnLStopLossService.DeleteEngineAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "PnL stop loss engine does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss global settings.</response>
        [HttpGet("/api/pnLStopLosses/globalSettings")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PnLStopLossEngineModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PnLStopLossSettingsModel>> GetAllGlobalSettingsAsync()
        {
            IReadOnlyCollection<PnLStopLossSettings> stopLossSettings = await _pnLStopLossService.GetAllGlobalSettingsAsync();

            return Mapper.Map<PnLStopLossSettingsModel[]>(stopLossSettings);
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss global settings successfully reapplied.</response>
        /// <response code="404">Stop loss global settings does not exist.</response>
        [HttpPut("/api/pnLStopLosses/reapplyGlobalSettings/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task ReapplyGlobalSettingsAsync(string id)
        {
            try
            {
                await _pnLStopLossService.ReapplyGlobalSettingsAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Stop loss global settings does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss successfully deleted.</response>
        /// <response code="400">An error occurred while deleting pnl stop loss.</response>
        /// <response code="404">Stop loss does not exist.</response>
        [HttpDelete("/engines/{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteGlobalSettingsAsync(string id)
        {
            try
            {
                await _pnLStopLossService.DeleteGlobalSettingsAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "PnL stop loss global settings does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss global settings successfully reapplied.</response>
        /// <response code="400">An error occurred while deleting pnl stop loss.</response>
        /// <response code="404">Stop loss global settings does not exist.</response>
        [HttpPut("/api/pnLStopLosses/update/{id}/{mode}")]
        public async Task UpdateEngineModeAsync(string id, PnLStopLossEngineMode mode)
        {
            if (mode == PnLStopLossEngineMode.None)
                throw new ValidationApiException(HttpStatusCode.BadRequest, $"'{nameof(mode)}' argument is not valid.");

            try
            {
                await _pnLStopLossService.UpdateEngineModeAsync(id, (Domain.PnLStopLossEngineMode) mode);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Stop loss global settings does not exist.");
            }
        }
    }
}
