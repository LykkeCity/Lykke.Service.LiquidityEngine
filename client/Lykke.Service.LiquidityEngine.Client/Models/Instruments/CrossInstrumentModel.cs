using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Instruments
{
    /// <summary>
    /// Represents a cross instrument that used to clone order book of main instrument.
    /// </summary>
    [PublicAPI]
    public class CrossInstrumentModel
    {
        /// <summary>
        /// The identifier of an internal asset pair.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Indicates that the asset pair is inverse.
        /// </summary>
        public bool IsInverse { get; set; }

        /// <summary>
        /// The name of external exchange that used as quote source.
        /// </summary>
        public string QuoteSource { get; set; }

        /// <summary>
        /// The identifier of an external asset pair.
        /// </summary>
        public string ExternalAssetPairId { get; set; }
    }
}
