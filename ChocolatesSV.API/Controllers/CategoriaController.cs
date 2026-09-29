using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class CategoriaController(ICategoriaService service) : ControllerBase
    {
        [HttpGet("api/categories")]
        [ProducesResponseType(typeof(IEnumerable<CategoriaDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetCategories()
        {
            var result = await service.GetAllCategoriesAsync();
            return Ok(result);
        }
    }
}