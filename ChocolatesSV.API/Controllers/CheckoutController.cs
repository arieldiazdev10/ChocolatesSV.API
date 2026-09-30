using System.Net;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class CheckoutController(IPaymentSimulationService service) : ControllerBase
    {
        [HttpPost("api/checkout/simulate-payment")]
        [ProducesResponseType(typeof(SimulatePaymentResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> SimulatePayment([FromBody] SimulatePaymentRequestDto request)
        {
            var result = await service.SimulateAsync(request);
            return result.Aprobado ? Ok(result) : BadRequest(result);
        }
    }
}
