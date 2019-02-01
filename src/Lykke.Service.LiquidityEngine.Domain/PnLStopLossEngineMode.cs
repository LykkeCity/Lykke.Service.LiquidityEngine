namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Specifies pnl stop loss engine modes.
    /// </summary>
    public enum PnLStopLossEngineMode
    {
        /// <summary>
        /// Unspecified mode.
        /// </summary>
        None,
        
        /// <summary>
        /// Indicates that the pnl stop loss engine is not used to calculate markup.
        /// </summary>
        Disabled,

        /// <summary>
        /// Indicates that the instrument can be used to calculate markup but threshold is not crossed.
        /// </summary>
        Idle,

        /// <summary>
        /// Indicates that the pnl stop loss engine is used to calculate markup.
        /// </summary>
        Active
    }
}
