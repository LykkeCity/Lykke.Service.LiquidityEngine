using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Positions;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class RemainingVolumesController : Controller, IRemainingVolumeApi
    {
        private readonly IRemainingVolumeService _remainingVolumeService;

        public RemainingVolumesController(IRemainingVolumeService remainingVolumeService)
        {
            _remainingVolumeService = remainingVolumeService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of remaining volumes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<RemainingVolumeModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<RemainingVolumeModel>> GetAllAsync()
        {
            IReadOnlyCollection<RemainingVolume> instruments = await _remainingVolumeService.GetAllAsync();

            return Mapper.Map<RemainingVolumeModel[]>(instruments);
        }
    }
}
