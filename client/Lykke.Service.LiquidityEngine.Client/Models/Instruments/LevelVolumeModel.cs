using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Instruments
{
    /// <summary>
    /// Represents a level volume of an instrument. 
    /// </summary>
    [PublicAPI]
    public class LevelVolumeModel
    {
        /// <summary>
        /// The number of the level.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// The volume of the limit order for this level.
        /// </summary>
        public decimal Volume { get; set; }
    }
}
