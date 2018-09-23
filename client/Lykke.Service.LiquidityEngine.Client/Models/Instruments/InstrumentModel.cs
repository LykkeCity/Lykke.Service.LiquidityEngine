using System.Collections.Generic;
using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Instruments
{
    /// <summary>
    /// Represents an instrument which used to create limit orders.
    /// </summary>
    [PublicAPI]
    public class InstrumentModel
    {
        /// <summary>
        /// The identifier of an internal asset pair. 
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Indicates this instrument is allowed to create limit orders.  
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The risk markup.
        /// </summary>
        public decimal Markup { get; set; }

        /// <summary>
        /// A collection of order book levels.
        /// </summary>
        public IReadOnlyCollection<LevelVolumeModel> Levels { get; set; }
    }
}
