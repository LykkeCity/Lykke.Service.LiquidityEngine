using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.CrossRateInstruments
{
    /// <summary>
    /// Represents a cross instrument that is used to convert prices.
    /// </summary>
    [PublicAPI]
    public class CrossRateInstrumentModel
    {
        /// <summary>
        /// The identifier of an internal asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

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
    }
}
