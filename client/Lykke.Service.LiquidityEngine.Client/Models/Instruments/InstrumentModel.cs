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
        /// The mode of the instrument.  
        /// </summary>
        public InstrumentMode Mode { get; set; }

        /// <summary>
        /// The threshold of the instrument realised profit and loss.
        /// </summary>
        public decimal PnLThreshold { get; set; }

        /// <summary>
        /// The threshold of the instrument absolute inventory.
        /// </summary>
        public decimal InventoryThreshold { get; set; }

        /// <summary>
        /// The accuracy of the hedge limit order volume that will be created on external exchange.
        /// </summary>
        public int VolumeAccuracy { get; set; }

        /// <summary>
        /// The min volume that can be used to create external limit order.
        /// </summary>
        public decimal MinVolume { get; set; }

        /// <summary>
        /// A collection of order book levels.
        /// </summary>
        public IReadOnlyCollection<InstrumentLevelModel> Levels { get; set; }
        
        /// <summary>
        /// A collection of cross instrument.
        /// </summary>
        public IReadOnlyCollection<CrossInstrumentModel> CrossInstruments { get; set; }
    }
}
