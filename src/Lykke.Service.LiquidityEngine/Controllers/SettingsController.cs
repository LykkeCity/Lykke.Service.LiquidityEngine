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
        private readonly ISettingsService _settingsService;
        private readonly ITimersSettingsService _timersSettingsService;
        private readonly IQuoteTimeoutSettingsService _quoteTimeoutSettingsService;
        private readonly IQuoteThresholdSettingsService _quoteThresholdSettingsService;
        private readonly IMarketMakerSettingsService _marketMakerSettingsService;

        public SettingsController(
            ISettingsService settingsService,
            ITimersSettingsService timersSettingsService,
            IQuoteTimeoutSettingsService quoteTimeoutSettingsService,
            IQuoteThresholdSettingsService quoteThresholdSettingsService,
            IMarketMakerSettingsService marketMakerSettingsService)
        {
            _settingsService = settingsService;
            _timersSettingsService = timersSettingsService;
            _quoteTimeoutSettingsService = quoteTimeoutSettingsService;
            _quoteThresholdSettingsService = quoteThresholdSettingsService;
            _marketMakerSettingsService = marketMakerSettingsService;
        }

        /// <inheritdoc />
        /// <response code="200">The settings of service account.</response>
        [HttpGet("account")]
        [ProducesResponseType(typeof(AccountSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<AccountSettingsModel> GetAccountSettingsAsync()
        {
            string walletId = await _settingsService.GetWalletIdAsync();

            return new AccountSettingsModel {WalletId = walletId};
        }

        /// <inheritdoc />
        /// <response code="200">The settings of market maker.</response>
        [HttpGet("marketmaker")]
        [ProducesResponseType(typeof(TimersSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<MarketMakerSettingsModel> GetMarketMakerSettingsAsync()
        {
            MarketMakerSettings marketMakerSettings = await _marketMakerSettingsService.GetAsync();

            return Mapper.Map<MarketMakerSettingsModel>(marketMakerSettings);
        }

        /// <inheritdoc />
        /// <response code="204">The settings of market maker successfully saved.</response>
        [HttpPost("marketmaker")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveMarketMakerSettingsAsync([FromBody] MarketMakerSettingsModel model)
        {
            var marketMakerSettings = Mapper.Map<MarketMakerSettings>(model);

            await _marketMakerSettingsService.UpdateAsync(marketMakerSettings);
        }

        /// <inheritdoc />
        /// <response code="200">The settings of service timers.</response>
        [HttpGet("timers")]
        [ProducesResponseType(typeof(TimersSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<TimersSettingsModel> GetTimersSettingsAsync()
        {
            TimersSettings timersSettings = await _timersSettingsService.GetAsync();

            return Mapper.Map<TimersSettingsModel>(timersSettings);
        }

        /// <inheritdoc />
        /// <response code="204">The settings of service timers successfully saved.</response>
        [HttpPost("timers")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveTimersSettingsAsync([FromBody] TimersSettingsModel model)
        {
            var timersSettings = Mapper.Map<TimersSettings>(model);

            await _timersSettingsService.SaveAsync(timersSettings);
        }

        /// <inheritdoc />
        /// <response code="200">The settings of quotes timeouts.</response>
        [HttpGet("quotes")]
        [ProducesResponseType(typeof(QuoteTimeoutSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<QuoteTimeoutSettingsModel> GetQuoteTimeoutSettingsAsync()
        {
            QuoteTimeoutSettings quoteTimeoutSettings = await _quoteTimeoutSettingsService.GetAsync();

            return Mapper.Map<QuoteTimeoutSettingsModel>(quoteTimeoutSettings);
        }

        /// <inheritdoc />
        /// <response code="204">The settings of quotes timeouts successfully saved.</response>
        [HttpPost("quotes")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveQuoteTimeoutSettingsAsync([FromBody] QuoteTimeoutSettingsModel model)
        {
            var quoteTimeoutSettings = Mapper.Map<QuoteTimeoutSettings>(model);

            await _quoteTimeoutSettingsService.SaveAsync(quoteTimeoutSettings);
        }

        /// <inheritdoc />
        /// <response code="200">The settings of quote threshold.</response>
        [HttpGet("quotes/threshold")]
        [ProducesResponseType(typeof(QuoteThresholdSettingsModel), (int) HttpStatusCode.OK)]
        public async Task<QuoteThresholdSettingsModel> GetQuoteThresholdSettingsAsync()
        {
            QuoteThresholdSettings quoteThresholdSettings = await _quoteThresholdSettingsService.GetAsync();

            return Mapper.Map<QuoteThresholdSettingsModel>(quoteThresholdSettings);
        }

        /// <inheritdoc />
        /// <response code="204">The settings of quote threshold successfully saved.</response>
        [HttpPost("quotes/threshold")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public async Task SaveQuoteThresholdSettingsAsync([FromBody] QuoteThresholdSettingsModel model)
        {
            var quoteThresholdSettings = Mapper.Map<QuoteThresholdSettings>(model);

            await _quoteThresholdSettingsService.UpdateAsync(quoteThresholdSettings);
        }
    }
}
