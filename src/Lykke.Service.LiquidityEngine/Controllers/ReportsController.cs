using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Lykke.Service.LiquidityEngine.Client.Api;
using Lykke.Service.LiquidityEngine.Client.Models.Reports;
using Microsoft.AspNetCore.Mvc;

namespace Lykke.Service.LiquidityEngine.Controllers
{
    [Route("/api/[controller]")]
    public class ReportsController : Controller, IReportsApi
    {
        public ReportsController()
        {
        }

        /// <inheritdoc/>
        /// <response code="200">A collection of asset pair summary info.</response>
        [HttpGet("summary")]
        [ProducesResponseType(typeof(IReadOnlyCollection<SummaryReportModel>), (int) HttpStatusCode.OK)]
        public Task<IReadOnlyCollection<SummaryReportModel>> GetSummaryReportAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
