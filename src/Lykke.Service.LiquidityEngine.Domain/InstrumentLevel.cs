namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a level of an instrument.
    /// </summary>
    public class InstrumentLevel
    {
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
