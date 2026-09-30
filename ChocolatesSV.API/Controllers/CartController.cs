using System.Net;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class CartController(ICartService service) : ControllerBase
    {
        [HttpPost("api/cart/calculate")]
        [ProducesResponseType(typeof(CalculateCartResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Conflict)]
        public async Task<IActionResult> Calculate([FromBody] CalculateCartRequestDto request)
        {
            try
            {
                return Ok(await service.CalculateAsync(request));
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
