using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class ContenidoController(IContenidoService service) : ControllerBase
    {
        [HttpGet("api/content/about")]
        [ProducesResponseType(typeof(InformacionEmpresaDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetAbout()
        {
            var result = await service.GetCompanyInfoAsync();
            return result != null ? Ok(result) : NotFound(new { message = "Información de la empresa no disponible" });
        }

        [HttpGet("api/content/faq")]
        [ProducesResponseType(typeof(IEnumerable<PreguntaFrecuenteDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFaq()
        {
            var result = await service.GetFaqAsync();
            return Ok(result);
        }
    }
}