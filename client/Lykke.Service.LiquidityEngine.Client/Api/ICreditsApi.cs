using System.Threading.Tasks;
using JetBrains.Annotations;
using Lykke.Service.LiquidityEngine.Client.Models.Credits;
using Refit;

namespace Lykke.Service.LiquidityEngine.Client.Api
{
    /// <summary>
    /// Provides methods to work with credits.
    /// </summary>
    [PublicAPI]
    public interface ICreditsApi
    {
        /// <summary>
        /// Changes credit amount of asset.
        /// </summary>
        /// <param name="model">The model which describes credit operation.</param>
        [Post("/api/credits")]
        Task SetAsync([Body] CreditOperationModel model);
    }
}
