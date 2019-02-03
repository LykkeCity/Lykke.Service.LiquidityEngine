namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Specifies internal order status.
    /// </summary>
    public enum InternalOrderStatus
    {
        /// <summary>
        /// Unspecified status.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that order created and waiting for process.
        /// </summary>
        New,

        /// <summary>
        /// Indicates that funds reserved to transfer.
        /// </summary>
        Reserved,

        /// <summary>
        /// Indicated that the order executed by the desired price or better.
        /// </summary>
        Executed,

        /// <summary>
        /// Indicates that funds transferred to destination wallet.
        /// </summary>
        Transferred,

        /// <summary>
        /// Indicated that the remaining funds transferred to destination wallet and order processing completed.
        /// </summary>
        Completed,

        /// <summary>
        /// Indicated that the order rejected. See rejection reason for more details.
        /// </summary>
        Rejected,

        /// <summary>
        /// Indicated that the order can not be executed by the desired price or an unexpected error occurred during processing.
        /// </summary>
        Failed,

        /// <summary>
        /// Indicated that the order cancelled and reserved funds transferred back to the destination wallet.
        /// </summary>
        Cancelled
    }
}
