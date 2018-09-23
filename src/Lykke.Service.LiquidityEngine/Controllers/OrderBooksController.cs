using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.OrderBooks;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class OrderBooksController : Controller, IOrderBooksApi
    {
        public OrderBooksController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of order books.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyCollection<OrderBookModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<OrderBookModel>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }

        /// <inheritdoc/>
        /// <response code="200">An order book.</response>
        [HttpGet("{assetPairId}")]
        [ProducesResponseType(typeof(OrderBookModel), (int) HttpStatusCode.OK)]
        public Task<OrderBookModel> GetByAssetPairAsync(string assetPairId)
        {
            throw new System.NotImplementedException();
        }
    }
}
