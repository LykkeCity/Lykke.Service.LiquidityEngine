using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossSettings;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class PnLStopLossSettingsController : Controller, IPnLStopLossSettingsApi
    {
        private readonly IPnLStopLossSettingsService _pnLStopLossSettingsService;

        public PnLStopLossSettingsController(IPnLStopLossSettingsService pnLStopLossService)
        {
            _pnLStopLossSettingsService = pnLStopLossService;
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss settings successfully added.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        public async Task AddAsync([FromBody] PnLStopLossSettingsModel pnLStopLossSettingsModel)
        {
            var stopLoss = Mapper.Map<PnLStopLossSettings>(pnLStopLossSettingsModel);

            await _pnLStopLossSettingsService.AddAsync(stopLoss);
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss settings.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PnLStopLossEngineModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PnLStopLossSettingsModel>> GetAllAsync()
        {
            IReadOnlyCollection<PnLStopLossSettings> stopLossSettings = await _pnLStopLossSettingsService.GetAllAsync();

            return Mapper.Map<PnLStopLossSettingsModel[]>(stopLossSettings);
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss settings successfully refreshed.</response>
        /// <response code="404">PnL stop loss settings does not exist.</response>
        [HttpPut("{id}/refresh")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task RefreshAsync(string id)
        {
            try
            {
                await _pnLStopLossSettingsService.RefreshAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "PnL stop loss settings does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss settings successfully deleted.</response>
        /// <response code="400">An error occurred while deleting pnl stop loss settings.</response>
        /// <response code="404">Stop loss settings does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _pnLStopLossSettingsService.DeleteAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "PnL stop loss settings does not exist.");
            }
            catch (InvalidOperationException exception)
            {
                throw new ValidationApiException(HttpStatusCode.BadRequest, exception.Message);
            }
        }
    }
}
