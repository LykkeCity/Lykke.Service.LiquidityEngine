using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settings;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with service settings.
    /// </summary>
    [PublicAPI]
    public interface ISettingsApi
    {
        /// <summary>
        /// Returns an account settings that used by service.
        /// </summary>
        /// <returns>The account settings.</returns>
        [Get("/api/settings/account")]
        Task<AccountSettingsModel> GetAccountSettingsAsync();

        /// <summary>
        /// Returns settings of market maker.
        /// </summary>
        /// <returns>The settings of market maker.</returns>
        [Get("/api/settings/marketmaker")]
        Task<MarketMakerSettingsModel> GetMarketMakerSettingsAsync();

        /// <summary>
        /// Saves settings of market maker.
        /// </summary>
        /// <param name="model">The settings of market maker.</param>
        [Post("/api/settings/marketmaker")]
        Task SaveMarketMakerSettingsAsync([Body] MarketMakerSettingsModel model);

        /// <summary>
        /// Returns settings of service timers.
        /// </summary>
        /// <returns>The settings of service timers.</returns>
        [Get("/api/settings/timers")]
        Task<TimersSettingsModel> GetTimersSettingsAsync();

        /// <summary>
        /// Saves settings of service timers.
        /// </summary>
        /// <param name="model">The settings of service timers.</param>
        [Post("/api/settings/timers")]
        Task SaveTimersSettingsAsync([Body] TimersSettingsModel model);

        /// <summary>
        /// Returns settings of quote timeouts.
        /// </summary>
        /// <returns>The settings of quote timeouts.</returns>
        [Get("/api/settings/quotes")]
        Task<QuoteTimeoutSettingsModel> GetQuoteTimeoutSettingsAsync();

        /// <summary>
        /// Saves settings of quote timeouts.
        /// </summary>
        /// <param name="model">The settings of quote timeouts.</param>
        [Post("/api/settings/quotes")]
        Task SaveQuoteTimeoutSettingsAsync([Body] QuoteTimeoutSettingsModel model);

        /// <summary>
        /// Returns settings of quote threshold.
        /// </summary>
        [Get("/api/settings/quotes/threshold")]
        Task<QuoteThresholdSettingsModel> GetQuoteThresholdSettingsAsync();

        /// <summary>
        /// Saves settings of quote threshold.
        /// </summary>
        /// <param name="model">The settings of quote threshold.</param>
        [Post("/api/settings/quotes/threshold")]
        Task SaveQuoteThresholdSettingsAsync([Body] QuoteThresholdSettingsModel model);
    }
}
