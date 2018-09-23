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
    }
}
