using System;
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
    public class PositionsController : Controller, IPositionsApi
    {
        private readonly IPositionService _positionService;

        public PositionsController(IPositionService positionService)
        {
            _positionService = positionService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetAllAsync(DateTime startDate, DateTime endDate, int limit)
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetAllAsync(startDate, endDate, limit);

            return Mapper.Map<PositionModel[]>(positions);
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet("open")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetOpenedAsync()
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetOpenedAsync();

            return Mapper.Map<PositionModel[]>(positions);
        }
        
        /// <inheritdoc/>
        /// <response code="200">A collection of positions.</response>
        [HttpGet("open/{assetPairId}")]
        [ProducesResponseType(typeof(IReadOnlyCollection<PositionModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<PositionModel>> GetOpenedByAssetPairIdAsync(string assetPairId)
        {
            IReadOnlyCollection<Position> positions = await _positionService.GetOpenedAsync(assetPairId);

            return Mapper.Map<PositionModel[]>(positions);
        }
    }
}
