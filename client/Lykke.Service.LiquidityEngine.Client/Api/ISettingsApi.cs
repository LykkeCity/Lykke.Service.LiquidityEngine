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
    }
}
