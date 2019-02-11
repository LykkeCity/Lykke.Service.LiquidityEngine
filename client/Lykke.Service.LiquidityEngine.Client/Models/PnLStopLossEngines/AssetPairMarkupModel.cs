using System;

namespace Lykke.Service.LiquidityEngine.Client.Models.PnLStopLossEngines
{
    /// <summary>
    /// Represents asset pair markup.
    /// </summary>
    [Obsolete("Use 'Markups.AssetPairMarkupModel' instead.")]
    public class AssetPairMarkupModel
    {
        /// <summary>
        /// The identifier of asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Total markup.
        /// </summary>
        public decimal TotalMarkup { get; set; }
    }
}
