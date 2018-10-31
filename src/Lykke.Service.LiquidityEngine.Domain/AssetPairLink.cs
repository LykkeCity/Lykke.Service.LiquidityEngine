namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a mapping between internal and external asset pairs.
    /// </summary>
    public class AssetPairLink
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

        public bool IsEmpty()
            => string.IsNullOrEmpty(AssetPairId) ||
               string.IsNullOrEmpty(ExternalAssetPairId) ||
               string.IsNullOrEmpty(ExternalBaseAssetId) ||
               string.IsNullOrEmpty(ExternalQuoteAssetId);

        public void Update(AssetPairLink assetPairLink)
        {
            ExternalAssetPairId = assetPairLink.ExternalAssetPairId;
            ExternalBaseAssetId = assetPairLink.ExternalBaseAssetId;
            ExternalQuoteAssetId = assetPairLink.ExternalQuoteAssetId;
        }
    }
}
