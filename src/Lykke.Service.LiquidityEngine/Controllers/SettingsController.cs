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

        public SettingsController(ITimersSettingsService timersSettingsService)
        {
            _timersSettingsService = timersSettingsService;
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
        [HttpGet("timers")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveTimersSettingsAsync([FromBody] TimersSettingsModel model)
        {
            var timersSettings = Mapper.Map<TimersSettings>(model);

            await _timersSettingsService.SaveAsync(timersSettings);
        }
    }
}
