using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.MarketMaker
{
    /// <summary>
    /// Market maker state to set
    /// </summary>
    [PublicAPI]
    public class MarketMakerStateUpdateModel
    {
        /// <summary>
        /// Market maker status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerStatus Status { get; set; }

        /// <summary>
        /// Comment on the reason to change state
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// User's identifier
        /// </summary>
        public string UserId { get; set; }
    }
}
