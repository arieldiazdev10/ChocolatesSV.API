using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class AuthController(IAuthService service) : ControllerBase
    {
        [HttpPost("api/admin/auth/login")]
        [ProducesResponseType(typeof(LoginResponseDto),
            (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var result = await service.LoginAsync(model);

            return result != null
                ? Ok(result)
                : Unauthorized(
                    new
                    {
                        message = "Credenciales inválidas"
                    });
        }
    }
}