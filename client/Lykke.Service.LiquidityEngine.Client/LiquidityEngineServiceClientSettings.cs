using JetBrains.Annotations;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Service.LiquidityEngine.Client
{
    /// <summary>
    /// Settings for liquidity engine service client.
    /// </summary>
    [PublicAPI]
    public class LiquidityEngineServiceClientSettings
    {
        /// <summary>Service url.</summary>
        [HttpCheck("api/isalive")]
        public string ServiceUrl { get; set; }
    }
}
