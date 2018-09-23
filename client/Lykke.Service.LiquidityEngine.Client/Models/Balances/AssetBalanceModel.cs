using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Balances
{
    /// <summary>
    /// Represent an asset balance details.
    /// </summary>
    [PublicAPI]
    public class AssetBalanceModel
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The current amount of balance including credit amount.
        /// </summary>
        public decimal Amount { get; set; }
        
        /// <summary>
        /// The amount of the credit.
        /// </summary>
        public decimal CreditAmount { get; set; }

        /// <summary>
        /// Indicates disbalance of current amount and credit amount.
        /// </summary>
        public decimal Disbalance { get; set; }
    }
}
