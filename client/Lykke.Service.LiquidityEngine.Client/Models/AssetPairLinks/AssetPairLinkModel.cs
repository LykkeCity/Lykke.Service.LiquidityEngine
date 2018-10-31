using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.AssetPairLinks
{
    /// <summary>
    /// Represents a mapping between internal and external asset pairs.
    /// </summary>
    [PublicAPI]
    public class AssetPairLinkModel
    {
        /// <summary>
        /// The identifier of asset pair which used for internal exchange.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// The identifier of asset pair which used for external exchange.
        /// </summary>
        public string ExternalAssetPairId { get; set; }

        /// <summary>
        /// The identifier of the base asset for external exchange.
        /// </summary>
        public string ExternalBaseAssetId { get; set; }

        /// <summary>
        /// The identifier of the base asset for external exchange.
        /// </summary>
        public string ExternalQuoteAssetId { get; set; }
    }
}
