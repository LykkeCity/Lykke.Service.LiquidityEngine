using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.InstrumentMarkups;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InstrumentMarkupsController : Controller, IInstrumentMarkupsApi
    {
        private readonly IInstrumentMarkupService _instrumentMarkupService;

        public InstrumentMarkupsController(IInstrumentMarkupService instrumentMarkupService)
        {
            _instrumentMarkupService = instrumentMarkupService;
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss engines.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentMarkupModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<InstrumentMarkupModel>> GetAllAsync()
        {
            IReadOnlyCollection<AssetPairMarkup> assetPairMarkups = await _instrumentMarkupService.GetAllAsync();

            return Mapper.Map<InstrumentMarkupModel[]>(assetPairMarkups);
        }
    }
}
