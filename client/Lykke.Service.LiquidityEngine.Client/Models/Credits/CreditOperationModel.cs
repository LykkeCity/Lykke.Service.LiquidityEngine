using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Credits
{
    /// <summary>
    /// Represent a credit details.
    /// </summary>
    [PublicAPI]
    public class CreditOperationModel
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The amount of a credit.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// The comment of the credit operation.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The identifier of the used which execute credit operation.
        /// </summary>
        public string UserId { get; set; }
    }
}
