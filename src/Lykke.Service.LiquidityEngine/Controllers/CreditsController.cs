using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Credits;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class CreditsController : Controller, ICreditsApi
    {
        public CreditsController()
        {
        }

        /// <inheritdoc/>
        /// <response code="204">The asset credit successfully changed.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task SetAsync([FromBody] CreditModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
