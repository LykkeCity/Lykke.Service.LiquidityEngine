using JetBrains.Annotations;

namespace Lykke.Service.LiquidityEngine.Client.Models.Settlements
{
    /// <summary>
    /// Represent a FIAT settlement operation details.
    /// </summary>
    [PublicAPI]
    public class FiatSettlementOperationModel
    {
        /// <summary>
        /// The identifier of settlement trade.
        /// </summary>
        public string SettlementTradeId { get; set; }
        
        /// <summary>
        /// The identifier of the used which execute settlement operation.
        /// </summary>
        public string UserId { get; set; }
    }
}
