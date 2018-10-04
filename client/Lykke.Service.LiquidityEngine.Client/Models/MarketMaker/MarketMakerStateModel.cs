using System;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Lykke.Service.LiquidityEngine.Client.Models.MarketMaker
{
    /// <summary>
    /// Market maker state
    /// </summary>
    [PublicAPI]
    public class MarketMakerStateModel
    {
        /// <summary>
        /// Market maker status
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerStatus Status { get; set; }

        /// <summary>
        /// Time since the status change
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Error identifier in case if market maker in error state
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public MarketMakerError Error { get; set; }

        /// <summary>
        /// Error description in case if market maker in error state
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
