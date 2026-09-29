using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    [Authorize]
    public class AdminOrdersController(IPedidoService service) : ControllerBase
    {
        [Authorize]
        [HttpGet("api/admin/orders")]
        [ProducesResponseType(typeof(IEnumerable<PedidoDto>), (int)HttpStatusCode.OK)]

        public async Task<IActionResult> GetAllOrders()
        {
            var result = await service.GetAllOrdersAsync(); return Ok(result);
        }
        [Authorize]
        [HttpGet("api/admin/orders/{id:int}")]
        [ProducesResponseType(typeof(PedidoDto), (int)HttpStatusCode.OK)]

        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await service.GetOrderByIdAsync(id);

            return result != null
                ? Ok(result)
                : NotFound();
        }
        [Authorize]
        [HttpPatch("api/admin/orders/{id:int}/status")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ActualizarEstadoPedidoDto model)
        {
            var result =
            await service.UpdateOrderStatusAsync(
            id,
            model.Estado);

            return result
            ? Ok(new { message = "Estado actualizado" })
            : NotFound(new
            {
                message = "Pedido no encontrado o estado inválido"
            });
        }
    }
}

     
  