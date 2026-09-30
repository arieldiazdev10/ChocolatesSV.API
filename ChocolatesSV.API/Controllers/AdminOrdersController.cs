using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class AdminOrdersController(IPedidoService service) : ControllerBase
    {
        [HttpGet("api/admin/orders")]
        [ProducesResponseType(typeof(IEnumerable<PedidoDto>), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> GetAllOrders()
        {
            var result = await service.GetAllOrdersAsync(); return Ok(result);
        }
    }
}
