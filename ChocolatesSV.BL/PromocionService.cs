using AutoMapper;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL
{
    public class PromocionService(IPromocionRepository promocionRepository, IMapper mapper) : IPromocionService
    {
        public async Task<List<PromocionDto>> GetActivePromotionsAsync()
        {
            var promociones = await promocionRepository.GetActivePromotionsAsync();
            return mapper.Map<List<PromocionDto>>(promociones);
        }

        public async Task<PromocionDto?> GetPromotionByIdAsync(int id)
        {
            var promocion = await promocionRepository.GetPromotionByIdAsync(id);
            return mapper.Map<PromocionDto?>(promocion);
        }

        public async Task<ValidarCuponResponseDto> ValidateCouponAsync(ValidarCuponRequestDto request)
        {
            var codigo = request.Codigo.Trim().ToUpper();
            var promocion = await promocionRepository.GetPromotionByCouponAsync(codigo);
            var hoy = DateTime.Now;

            if (promocion == null || promocion.TipoPromocion != "Cupon")
                return Invalido("El cupón no existe");

            if (!promocion.Activo)
                return Invalido("El cupón no está activo");

            if (hoy < promocion.FechaInicio)
                return Invalido("El cupón todavía no está vigente");

            if (hoy.Date > promocion.FechaFin.Date)
                return Invalido("El cupón ha expirado");

            if (promocion.UsosMaximos.HasValue && promocion.UsosActuales >= promocion.UsosMaximos.Value)
                return Invalido("El cupón ya alcanzó su límite de usos");

            if (promocion.MontoMinimoCompra.HasValue && request.Subtotal < promocion.MontoMinimoCompra.Value)
                return Invalido($"El cupón requiere una compra mínima de ${promocion.MontoMinimoCompra.Value:0.00}");

            var descuento = promocion.TipoDescuento == "Porcentaje"
                ? Math.Round(request.Subtotal * promocion.ValorDescuento / 100, 2)
                : promocion.ValorDescuento;

            // El descuento nunca puede ser mayor al subtotal
            descuento = Math.Min(descuento, request.Subtotal);

            return new ValidarCuponResponseDto
            {
                Valido = true,
                Mensaje = "Cupón aplicado correctamente",
                Codigo = promocion.CodigoCupon,
                TipoDescuento = promocion.TipoDescuento,
                Valor = promocion.ValorDescuento,
                MontoDescuento = descuento,
                TotalConDescuento = request.Subtotal - descuento
            };
        }

        public async Task<PromocionDto> InsertPromotionAsync(PromocionDto promocion)
        {
            await ValidateBusinessRulesAsync(promocion, null);
            var entity = mapper.Map<Promocion>(promocion);
            var newId = await promocionRepository.InsertPromotionAsync(entity);
            promocion.Id = newId;
            return promocion;
        }

        public async Task<PromocionDto?> UpdatePromotionAsync(int id, PromocionDto promocion)
        {
            await ValidateBusinessRulesAsync(promocion, id);
            var entity = mapper.Map<Promocion>(promocion);
            entity.PromocionID = id;
            var updated = await promocionRepository.UpdatePromotionAsync(entity);
            if (!updated)
            {
                return null;
            }
            promocion.Id = id;
            return promocion;
        }

        public async Task<bool> DeletePromotionAsync(int id)
        {
            return await promocionRepository.DeletePromotionAsync(id);
        }

        // ---------- Reglas de negocio ----------

        private async Task ValidateBusinessRulesAsync(PromocionDto promocion, int? id)
        {
            if (promocion.FechaFin < promocion.FechaInicio)
                throw new ArgumentException("La fecha de fin no puede ser anterior a la fecha de inicio");

            if (promocion.TipoDescuento == "Porcentaje" && promocion.Descuento > 100)
                throw new ArgumentException("Un descuento en porcentaje no puede ser mayor a 100");

            if (promocion.Productos.GroupBy(p => p.IdProducto).Any(g => g.Count() > 1))
                throw new ArgumentException("No se puede repetir el mismo producto en la promoción");

            switch (promocion.Tipo)
            {
                case "Cupon":
                    if (string.IsNullOrWhiteSpace(promocion.Cupon))
                        throw new ArgumentException("Una promoción de tipo Cupon necesita un código");
                    promocion.Cupon = promocion.Cupon.Trim().ToUpper();
                    if (await promocionRepository.CouponExistsAsync(promocion.Cupon, id))
                        throw new ArgumentException($"Ya existe un cupón con el código {promocion.Cupon}");
                    break;

                case "Combo":
                    if (promocion.PrecioCombo == null)
                        throw new ArgumentException("Un combo necesita el precio del combo");
                    if (promocion.Productos.Count < 2)
                        throw new ArgumentException("Un combo necesita al menos 2 productos");
                    break;

                case "Producto":
                    if (promocion.Productos.Count == 0)
                        throw new ArgumentException("Debe indicar al menos un producto");
                    break;

                case "Categoria":
                    if (promocion.IdCategoria == null)
                        throw new ArgumentException("Debe indicar la categoría");
                    break;
            }

            // Limpia los campos que no aplican al tipo elegido
            if (promocion.Tipo != "Cupon") promocion.Cupon = null;
            if (promocion.Tipo != "Combo") promocion.PrecioCombo = null;
            if (promocion.Tipo != "Categoria") promocion.IdCategoria = null;
        }

        private static ValidarCuponResponseDto Invalido(string mensaje) => new()
        {
            Valido = false,
            Mensaje = mensaje
        };
    }
}
