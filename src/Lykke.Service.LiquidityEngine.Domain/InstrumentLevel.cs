namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a level of an instrument.
    /// </summary>
    public class InstrumentLevel
    {
        private decimal _volume;

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
        public decimal Volume
        {
            get => _volume;
            set { _volume = value;
                BuyVolume = value;
                SellVolume = value;
            }
        }

        /// <summary>
        /// The risk markup.
        /// </summary>
        public decimal Markup { get; set; }

        /// <summary>
        /// Remaining volume of the limit order for buy level.
        /// </summary>
        public decimal BuyVolume { get; set; }

        /// <summary>
        /// Remaining volume of the limit order for sell level.
        /// </summary>
        public decimal SellVolume { get; set; }
    }
}
