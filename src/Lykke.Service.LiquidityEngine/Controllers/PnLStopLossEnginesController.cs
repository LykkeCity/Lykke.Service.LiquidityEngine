using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class PnLStopLossEnginesController : Controller, IPnLStopLossEnginesApi
    {
        private readonly IPnLStopLossEngineService _pnLStopLossEngineService;

        public PnLStopLossEnginesController(IPnLStopLossEngineService pnLStopLossEngineService)
        {
            _pnLStopLossEngineService = pnLStopLossEngineService;
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss engines.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PnLStopLossEngineModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PnLStopLossEngineModel>> GetAllAsync()
        {
            IReadOnlyCollection<PnLStopLossEngine> stopLosses = await _pnLStopLossEngineService.GetAllAsync();

            return Mapper.Map<PnLStopLossEngineModel[]>(stopLosses);
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss engines successfully updated.</response>
        /// <response code="400">An error occurred while update pnl stop loss engine.</response>
        /// <response code="404">Stop loss engine does not exist.</response>
        [HttpPut]
        public async Task UpdateAsync([FromBody] PnLStopLossEngineModel pnLStopLossEngineModel)
        {
            try
            {
                var pnLStopLossEngine = Mapper.Map<PnLStopLossEngine>(pnLStopLossEngineModel);

                await _pnLStopLossEngineService.UpdateAsync(pnLStopLossEngine);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Stop loss engine does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The pnl stop loss engine successfully deleted.</response>
        /// <response code="400">An error occurred while deleting pnl stop loss engine.</response>
        /// <response code="404">Stop loss engine does not exist.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string id)
        {
            try
            {
                await _pnLStopLossEngineService.DeleteAsync(id);
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
    }
}
