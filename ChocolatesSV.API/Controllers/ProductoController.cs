using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class ProductoController(IProductoService service) : ControllerBase
    {
        [HttpGet("api/products")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveProducts()
        {
            var result = await service.GetAllActiveProductsAsync();
            return Ok(result);
        }

        [HttpGet("api/products/featured")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var result = await service.GetFeaturedProductsAsync();
            return Ok(result);
        }

        [HttpGet("api/products/{id:int}")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetProductByIdAsync(id);
            return result != null ? Ok(result) : NotFound(new { message = "Producto no encontrado" });
        }

        [Authorize]
        [HttpPost("api/admin/products")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Post([FromBody] ProductoDto model)
        {
            var result = await service.InsertProductAsync(model);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPut("api/admin/products/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidationProblemDetails), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductoDto model)
        {
            var result = await service.UpdateProductAsync(id, model);
            return result != null
                ? Ok(new { message = "Producto actualizado" })
                : NotFound(new { message = "Producto no encontrado para actualizar" });
        }

        [Authorize]
        [HttpDelete("api/admin/products/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var result = await service.SoftDeleteProductAsync(id);
            return result
                ? Ok(new { message = "Producto eliminado" })
                : NotFound(new { message = "Producto no encontrado para eliminar" });
        }
    }
}