using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    [Authorize]
    public class DashboardController(IDashboardService service) : ControllerBase
    {
        // GET: api/admin/dashboard/summary
        [HttpGet("api/admin/dashboard/summary")]
        [ProducesResponseType(typeof(DashboardSummaryDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetSummary()
        {
            var result = await service.GetSummaryAsync();
            return Ok(result);
        }
    }
}