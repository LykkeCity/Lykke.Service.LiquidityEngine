using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Common.Api.Contract.Responses;
using Lykke.Common.ApiLibrary.Exceptions;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Exceptions;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class CrossRateInstrumentsController : Controller, ICrossRateInstrumentsApi
    {
        private readonly ICrossRateInstrumentService _crossRateInstrumentService;

        public CrossRateInstrumentsController(ICrossRateInstrumentService crossRateInstrumentService)
        {
            _crossRateInstrumentService = crossRateInstrumentService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of cross-rate instruments.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<CrossRateInstrumentModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<CrossRateInstrumentModel>> GetAllAsync()
        {
            IReadOnlyCollection<CrossRateInstrument> instruments = await _crossRateInstrumentService.GetAllAsync();

            return Mapper.Map<List<CrossRateInstrumentModel>>(instruments);
        }

        /// <inheritdoc/>
        /// <response code="200">An instrument.</response>
        /// <response code="404">Cross-rate instrument does not exist.</response>
        [HttpGet("{assetPairId}")]
        [ProducesResponseType(typeof(CrossRateInstrumentModel), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task<CrossRateInstrumentModel> GetByAssetPairIdAsync(string assetPairId)
        {
            try
            {
                CrossRateInstrument crossInstrument =
                    await _crossRateInstrumentService.GetByAssetPairIdAsync(assetPairId);

                return Mapper.Map<CrossRateInstrumentModel>(crossInstrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Cross-rate instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully added.</response>
        /// <response code="409">Instrument already exists.</response>
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.Conflict)]
        public async Task AddAsync([FromBody] CrossRateInstrumentModel model)
        {
            try
            {
                var crossInstrument = Mapper.Map<CrossRateInstrument>(model);

                await _crossRateInstrumentService.AddAsync(crossInstrument);
            }
            catch (EntityAlreadyExistsException)
            {
                throw new ValidationApiException(HttpStatusCode.Conflict, "Cross-rate instrument already exists.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully updated.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpPut]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task UpdateAsync([FromBody] CrossRateInstrumentModel model)
        {
            try
            {
                var crossInstrument = Mapper.Map<CrossRateInstrument>(model);

                await _crossRateInstrumentService.UpdateAsync(crossInstrument);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Cross-rate instrument does not exist.");
            }
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully deleted.</response>
        /// <response code="404">Instrument does not exist.</response>
        [HttpDelete("{assetPairId}")]
        [ProducesResponseType((int)HttpStatusCode.NoContent)]
        [ProducesResponseType(typeof(ErrorResponse), (int)HttpStatusCode.NotFound)]
        public async Task DeleteAsync(string assetPairId)
        {
            try
            {
                await _crossRateInstrumentService.DeleteAsync(assetPairId);
            }
            catch (EntityNotFoundException)
            {
                throw new ValidationApiException(HttpStatusCode.NotFound, "Cross-rate instrument does not exist.");
            }
        }
    }
}
