using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    [Authorize]
    public class AdminOrdersController(IPedidoService service) : ControllerBase
    {
        [HttpGet("api/admin/orders")]
        [ProducesResponseType(typeof(IEnumerable<PedidoDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> GetAllOrders([FromQuery] PedidoFiltroDto filtro)
        {
            try
            {
                var result = await service.GetAllOrdersAsync(filtro);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("api/admin/orders/{id:int}")]
        [ProducesResponseType(typeof(PedidoCompletoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var result = await service.GetOrderByIdAsync(id);
            return result != null ? Ok(result) : NotFound(new { message = "Pedido no encontrado" });
        }

        [HttpPatch("api/admin/orders/{id:int}/status")]
        [ProducesResponseType(typeof(PedidoCompletoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] ActualizarEstadoPedidoDto request)
        {
            try
            {
                var result = await service.UpdateOrderStatusAsync(id, request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}