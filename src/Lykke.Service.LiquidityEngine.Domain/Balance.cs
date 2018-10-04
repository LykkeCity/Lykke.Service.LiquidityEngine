namespace Lykke.Service.LiquidityEngine.Domain
{
    /// <summary>
    /// Represents a balance of an asset.
    /// </summary>
    public class Balance
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Balance"/> of asset with amount of balance.
        /// </summary>
        /// <param name="assetId">The asset id.</param>
        /// <param name="amount">The amount of balance.</param>
        public Balance(string assetId, decimal amount)
        {
            AssetId = assetId;
            Amount = amount;
        }
        
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; }
        
        /// <summary>
        /// The amount of balance.
        /// </summary>
        public decimal Amount { get; }
    }
}
