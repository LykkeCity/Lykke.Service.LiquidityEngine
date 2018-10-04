using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Audit;
using Lykke.Service.LiquidityEngine.Domain;
using Lykke.Service.LiquidityEngine.Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class AuditController : Controller, IAuditApi
    {
        private readonly IBalanceOperationService _balanceOperationService;

        public AuditController(IBalanceOperationService balanceOperationService)
        {
            _balanceOperationService = balanceOperationService;
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of balance operations.</response>
        [HttpGet("balances")]
        [ProducesResponseType(typeof(IReadOnlyCollection<BalanceOperationModel>), (int) HttpStatusCode.OK)]
        public async Task<IReadOnlyCollection<BalanceOperationModel>> GetBalanceOperationsAsync(DateTime startDate,
            DateTime endDate, int limit)
        {
            IReadOnlyCollection<BalanceOperation> balanceOperations =
                await _balanceOperationService.GetAsync(startDate, endDate, limit);

            return Mapper.Map<List<BalanceOperationModel>>(balanceOperations);
        }
    }
}
