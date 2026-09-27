using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using global::ChocolatesSV.BL.Interfaces;
using global::ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class TrackingController(IPedidoService service) : ControllerBase
    {
        [HttpPost("api/orders/tracking")]
        [ProducesResponseType(typeof(TrackingResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Track([FromBody] TrackingRequestDto model)

        {
            var result = await service.TrackOrderAsync(model.PedidoId, model.Correo);

            return result != null
                ? Ok(result)
                : NotFound(
                    new
                    {
                        message = "Pedido o correo inválido"
                    });
        }
    }
}