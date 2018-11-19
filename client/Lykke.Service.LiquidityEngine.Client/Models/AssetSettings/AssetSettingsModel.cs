using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.AssetSettings
{
    /// <summary>
    /// Represents a settings set that describes an asset.
    /// </summary>
    [PublicAPI]
    public class AssetSettingsModel
    {
        /// <summary>
        /// The identifier of an asset.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The identifier of the linked Lykke asset.
        /// </summary>
        public string LykkeAssetId { get; set; }

        /// <summary>
        /// The name of external exchange that used as quote source.
        /// </summary>
        public string QuoteSource { get; set; }

        /// <summary>
        /// The identifier of an external asset pair.
        /// </summary>
        public string ExternalAssetPairId { get; set; }

        /// <summary>
        /// Indicates that the external asset pair is inverse.
        /// </summary>
        public bool IsInverse { get; set; }

        /// <summary>
        /// The accuracy of the asset used for display purposes.
        /// </summary>
        public int DisplayAccuracy { get; set; }

        /// <summary>
        /// The flag that specifies if the asset is cryptocurrency.
        /// </summary>
        public bool IsCrypto { get; set; }
    }
}
