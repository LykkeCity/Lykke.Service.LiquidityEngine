using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class AuditController : Controller, IAuditApi
    {
        public AuditController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of balance operations.</response>
        [HttpGet("balances")]
        [ProducesResponseType(typeof(IReadOnlyCollection<BalanceOperationModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<BalanceOperationModel>> GetBalanceOperationsAsync(DateTime startDate, DateTime endDate, int limit)
        {
            throw new NotImplementedException();
        }
    }
}
