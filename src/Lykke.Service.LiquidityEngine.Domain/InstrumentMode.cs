namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Specifies an instrument mode.
    /// </summary>
    public enum InstrumentMode
    {
        /// <summary>
        /// Unspecified mode.
        /// </summary>
        None,
        
        /// <summary>
        /// Indicates that the instrument is not used to calculate order book.
        /// </summary>
        Disabled,
        
        /// <summary>
        /// Indicates that the instrument is used to calculate order book but limit orders not created.
        /// </summary>
        Idle,
        
        /// <summary>
        /// Indicates that the instrument is used to calculate order book and limit orders created.
        /// </summary>
        Active
    }
}
