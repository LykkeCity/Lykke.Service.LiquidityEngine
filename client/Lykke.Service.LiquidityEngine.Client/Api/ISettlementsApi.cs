using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with settlements.
    /// </summary>
    [PublicAPI]
    public interface ISettlementsApi
    {
        /// <summary>
        /// Execute settlement for the asset.
        /// </summary>
        /// <param name="model">The model which describes settlement.</param>
        [Post("/api/settlements")]
        Task SettlementAsync([Body] SettlementOperationModel model);
        
        /// <summary>
        /// Execute fiat settlement.
        /// </summary>
        /// <param name="model">The model which describes fiat settlement.</param>
        [Post("/api/settlements/fiat")]
        Task SettlementAsync([Body] FiatSettlementOperationModel model);
    }
}
