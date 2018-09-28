using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Settings;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class SettingsController : Controller, ISettingsApi
    {
        private readonly ITimersSettingsService _timersSettingsService;
        private readonly IQuoteTimeoutSettingsService _quoteTimeoutSettingsService;

        public SettingsController(
            ITimersSettingsService timersSettingsService,
            IQuoteTimeoutSettingsService quoteTimeoutSettingsService)
        {
            _timersSettingsService = timersSettingsService;
            _quoteTimeoutSettingsService = quoteTimeoutSettingsService;
        }

        /// <response code="200">The settings of service timers.</response>
        [HttpGet("timers")]
        [ProducesResponseType(typeof(TimersSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<TimersSettingsModel> GetTimersSettingsAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return Mapper.Map<TimersSettingsModel>(timersSettings);
        }

        /// <response code="204">The settings of service timers successfully saved.</response>
        [HttpPost("timers")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveTimersSettingsAsync([FromBody] TimersSettingsModel model)
        {
            var timersSettings = Mapper.Map<TimersSettings>(model);

            await _timersSettingsService.SaveAsync(timersSettings);
        }

        /// <response code="200">The settings of quotes timeouts.</response>
        [HttpGet("quotes")]
        [ProducesResponseType(typeof(QuoteTimeoutSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<QuoteTimeoutSettingsModel> GetQuoteTimeoutSettingsAsync()
        {
            QuoteTimeoutSettings quoteTimeoutSettings = await _quoteTimeoutSettingsService.GetAsync();

            return Mapper.Map<QuoteTimeoutSettingsModel>(quoteTimeoutSettings);
        }

        /// <response code="204">The settings of quotes timeouts successfully saved.</response>
        [HttpPost("quotes")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveQuoteTimeoutSettingsAsync(QuoteTimeoutSettingsModel model)
        {
            var quoteTimeoutSettings = Mapper.Map<QuoteTimeoutSettings>(model);

            await _quoteTimeoutSettingsService.SaveAsync(quoteTimeoutSettings);
        }
    }
}
