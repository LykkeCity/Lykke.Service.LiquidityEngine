using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.InstrumentMessages;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class InstrumentMessagesController : Controller, IInstrumentMessagesApi
    {
        private readonly IInstrumentMessagesService _instrumentMessagesService;

        public InstrumentMessagesController(IInstrumentMessagesService instrumentMessagesService)
        {
            _instrumentMessagesService = instrumentMessagesService;
        }

        /// <inheritdoc/>
        /// <response code="200">Collection of pnl stop loss engines.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<InstrumentMessagesModel>), (int)HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<InstrumentMessagesModel>> GetAllAsync()
        {
            IReadOnlyCollection<InstrumentMessages> instrumentsMessages = await _instrumentMessagesService.GetAllAsync();

            return Mapper.Map<InstrumentMessagesModel[]>(instrumentsMessages);
        }
    }
}
