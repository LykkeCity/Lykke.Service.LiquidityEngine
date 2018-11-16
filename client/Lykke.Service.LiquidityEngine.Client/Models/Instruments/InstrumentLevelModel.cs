using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Instruments
{
    /// <summary>
    /// Represents a level of an instrument. 
    /// </summary>
    [PublicAPI]
    public class InstrumentLevelModel
    {
        /// <summary>
        /// The identifier of the level.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The number of the level.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The volume of the limit order for this level.
        /// </summary>
        public decimal Volume { get; set; }
        
        /// <summary>
        /// The risk markup.
        /// </summary>
        public decimal Markup { get; set; }
    }
}
