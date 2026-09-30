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

        // GET: api/admin/dashboard/top-products?top=5&desde=2026-09-01&hasta=2026-09-30
        [HttpGet("api/admin/dashboard/top-products")]
        [ProducesResponseType(typeof(IEnumerable<TopProductoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetTopProducts(
            [FromQuery] int top = 5,
            [FromQuery] DateTime? desde = null,
            [FromQuery] DateTime? hasta = null)
        {
            try
            {
                var result = await service.GetTopProductsAsync(top, desde, hasta);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}