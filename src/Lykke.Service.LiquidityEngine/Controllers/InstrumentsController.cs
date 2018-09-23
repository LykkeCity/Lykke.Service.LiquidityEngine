using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Instruments;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InstrumentsController : Controller, IInstrumentsApi
    {
        public InstrumentsController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of instruments.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<InstrumentModel>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">An instrument.</response>
        [HttpGet("{assetPairId}")]
        [ProducesResponseType(typeof(InstrumentModel), (int) HttpStatusCode.OK)]
        public Task<InstrumentModel> GetByAssetPairIdAsync(string assetPairId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully added.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task AddAsync([FromBody] InstrumentModel model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully updated.</response>
        [HttpPut]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task UpdateAsync([FromBody] InstrumentModel model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The instrument successfully deleted.</response>
        [HttpDelete("{assetPairId}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task DeleteAsync(string assetPairId)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully added to instrument.</response>
        [HttpPost("{assetPairId}/levels")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task AddLevelAsync(string assetPairId, [FromBody] LevelVolumeModel model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully updated.</response>
        [HttpPut("{assetPairId}/levels")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task UpdateLevelAsync(string assetPairId, [FromBody] LevelVolumeModel model)
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="204">The volume level successfully removed from instrument.</response>
        [HttpDelete("{assetPairId}/levels/{levelNumber}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task RemoveLevelAsync(string assetPairId, int levelNumber)
        {
            throw new System.NotImplementedException();
        }
    }
}
