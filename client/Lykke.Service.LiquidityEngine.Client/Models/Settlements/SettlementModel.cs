using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settlements
{
    /// <summary>
    /// Represent a settlement details.
    /// </summary>
    [PublicAPI]
    public class SettlementModel
    {
        /// <summary>
        /// The asset id.
        /// </summary>
        public string AssetId { get; set; }

        /// <summary>
        /// The amount of a settlement.
        /// </summary>
        public decimal Amount { get; set; }
    }
}
