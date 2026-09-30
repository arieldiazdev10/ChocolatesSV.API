using System.Net;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class OrdersController(IPedidoService service) : ControllerBase
    {
        [HttpPost("api/orders")]
        [ProducesResponseType(typeof(CreateOrderResponseDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Create([FromBody] CreateOrderRequestDto request)
        {
            try
            {
                var result = await service.CreateOrderAsync(request);
                return StatusCode((int)HttpStatusCode.Created, result);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("api/orders/tracking")]
        [ProducesResponseType(typeof(TrackOrderResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Track([FromBody] TrackOrderRequestDto request)
        {
            try
            {
                var result = await service.TrackOrderAsync(request);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}