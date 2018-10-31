using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settlements
{
    /// <summary>
    /// Represent a settlement operation details.
    /// </summary>
    [PublicAPI]
    public class SettlementOperationModel
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The amount of a settlement.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// If <c>true</c> then cash-in/out operation will be executed while settlement, otherwise not.
        /// </summary>
        public bool AllowChangeBalance { get; set; }

        /// <summary>
        /// The comment of the settlement operation.
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// The identifier of the used which execute settlement operation.
        /// </summary>
        public string UserId { get; set; }
    }
}
