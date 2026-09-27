using ChocolatesSV.BL.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class ContentController(IContentService service)
        : ControllerBase
    {
        [HttpGet("api/content/about")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> About()
        {
            var result = await service.GetAboutAsync();

            return Ok(result);
        }

        [HttpGet("api/content/faq")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Faq()
        {
            var result = await service.GetFaqAsync();

            return Ok(result);
        }
    }
}