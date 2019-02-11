using System.Collections.Generic;

namespace Lykke.Service.LiquidityEngine.Client.Models.InstrumentMessages
{
    /// <summary>
    /// Represents instrument messages.
    /// </summary>
    public class InstrumentMessagesModel
    {
        /// <summary>
        /// Asset pair id.
        /// </summary>
        public string AssetPairId { get; set; }

        /// <summary>
        /// Instrument messages.
        /// </summary>
        public IReadOnlyCollection<string> Messages { get; set; }
    }
}
