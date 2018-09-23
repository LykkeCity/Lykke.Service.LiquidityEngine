using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Settlements;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class SettlementsController : Controller, ISettlementsApi
    {
        public SettlementsController()
        {
        }

        /// <inheritdoc/>
        /// <response code="204">The settlement successfully executed.</response>
        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        public Task SettlementAsync([FromBody] SettlementModel model)
        {
            throw new System.NotImplementedException();
        }
    }
}
