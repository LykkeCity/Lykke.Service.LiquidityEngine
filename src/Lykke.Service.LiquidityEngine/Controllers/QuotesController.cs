using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Quotes;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class QuotesController : Controller, IQuotesApi
    {
        private readonly IQuoteService _quoteService;

        public QuotesController(IQuoteService quoteService)
        {
            _quoteService = quoteService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of quotes.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<QuoteModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<QuoteModel>> GetAllAsync()
        {
            IReadOnlyCollection<Quote> quotes = await _quoteService.GetAsync();

            return Mapper.Map<QuoteModel[]>(quotes);
        }
    }
}
