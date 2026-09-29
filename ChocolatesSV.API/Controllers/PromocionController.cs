using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ChocolatesSV.API.Controllers
{
    [ApiController]
    public class PromocionController(IPromocionService service) : ControllerBase
    {
        // GET: api/promotions/active
        [HttpGet("api/promotions/active")]
        [ProducesResponseType(typeof(IEnumerable<PromocionDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetActivePromotions()
        {
            var result = await service.GetActivePromotionsAsync();
            return Ok(result);
        }

        // POST: api/promotions/validate-coupon
        [HttpPost("api/promotions/validate-coupon")]
        [ProducesResponseType(typeof(ValidarCuponResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ValidarCuponResponseDto), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> ValidateCoupon([FromBody] ValidarCuponRequestDto model)
        {
            var result = await service.ValidateCouponAsync(model);
            return result.Valido ? Ok(result) : BadRequest(result);
        }

        // GET: api/admin/promotions/{id}  (apoyo para CreatedAtAction y para el formulario de edición)
        [Authorize]
        [HttpGet("api/admin/promotions/{id:int}")]
        [ProducesResponseType(typeof(PromocionDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await service.GetPromotionByIdAsync(id);
            return result != null ? Ok(result) : NotFound();
        }

        // POST: api/admin/promotions
        [Authorize]
        [HttpPost("api/admin/promotions")]
        [ProducesResponseType(typeof(PromocionDto), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] PromocionDto model)
        {
            try
            {
                var result = await service.InsertPromotionAsync(model);
                return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/admin/promotions/{id}
        [Authorize]
        [HttpPut("api/admin/promotions/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] PromocionDto model)
        {
            try
            {
                var result = await service.UpdatePromotionAsync(id, model);
                return result != null
                    ? Ok(new { message = "Promoción actualizada" })
                    : NotFound(new { message = "Promoción no encontrada para actualizar" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/admin/promotions/{id}
        [Authorize]
        [HttpDelete("api/admin/promotions/{id:int}")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeletePromotionAsync(id);
            return result
                ? Ok(new { message = "Promoción eliminada" })
                : NotFound(new { message = "Promoción no encontrada para eliminar" });
        }
    }
}