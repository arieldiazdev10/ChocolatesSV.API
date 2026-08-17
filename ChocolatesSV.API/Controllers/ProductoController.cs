using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class ProductoController(IProductoService service) : ControllerBase
    {
        // GET: api/products
        [HttpGet("api/products")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActiveProducts()
        {
            var result = await service.GetAllActiveProductsAsync();
            return Ok(result);
        }

        // GET: api/products/featured
        [HttpGet("api/products/featured")]
        [ProducesResponseType(typeof(IEnumerable<ProductoDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetFeaturedProducts()
        {
            var result = await service.GetFeaturedProductsAsync();
            return Ok(result);
        }

        // GET: api/products/{id}
        [HttpGet("api/products/{id:int}")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetProductByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        // POST: api/admin/products
        [HttpPost("api/admin/products")]
        [ProducesResponseType(typeof(ProductoDto), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Post([FromBody] ProductoDto model)
        {
            var result = await service.InsertProductAsync(model);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        // PUT: api/admin/products/{id}
        [HttpPut("api/admin/products/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] ProductoDto model)
        {
            var result = await service.UpdateProductAsync(id, model);
            return result != null
                ? Ok(new { message = "Producto actualizado" })
                : NotFound(new { message = "Producto no encontrado para actualizar" });
        }

        // DELETE: api/admin/products/{id}
        [HttpDelete("api/admin/products/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
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