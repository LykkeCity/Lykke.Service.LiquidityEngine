using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Quotes;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class QuotesController : Controller, IQuotesApi
    {
        public QuotesController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of quotes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<QuoteModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<QuoteModel>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
