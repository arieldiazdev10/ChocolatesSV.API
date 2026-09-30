using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL
{
    public class CartService(
        IProductoRepository productoRepository,
        IPromocionRepository promocionRepository) : ICartService
    {
        public async Task<CalculateCartResponseDto> CalculateAsync(CalculateCartRequestDto request)
        {
            if (request.Items.Count == 0)
                throw new ArgumentException("El carrito debe contener al menos un producto");

            var requestedItems = request.Items
                .GroupBy(item => item.ProductoId)
                .Select(group => new CartItemRequestDto
                {
                    ProductoId = group.Key,
                    Cantidad = group.Sum(item => item.Cantidad)
                })
                .ToList();

            if (requestedItems.Any(item => item.Cantidad <= 0))
                throw new ArgumentException("Todas las cantidades deben ser mayores que cero");

            var products = await productoRepository.GetProductsByIdsAsync(requestedItems.Select(item => item.ProductoId));
            if (products.Count != requestedItems.Count)
            {
                var foundIds = products.Select(product => product.ProductoID).ToHashSet();
                var missingId = requestedItems.First(item => !foundIds.Contains(item.ProductoId)).ProductoId;
                throw new KeyNotFoundException($"El producto {missingId} no existe o está inactivo");
            }

            var promotions = await promocionRepository.GetActivePromotionsAsync();
            var response = new CalculateCartResponseDto();
            var subtotal = 0m;

            foreach (var item in requestedItems)
            {
                var product = products.Single(product => product.ProductoID == item.ProductoId);
                if (product.Existencias < item.Cantidad)
                    throw new InvalidOperationException($"Stock insuficiente para el producto {product.Nombre}");

                var lineSubtotal = Round(product.Precio * item.Cantidad);
                subtotal += lineSubtotal;
                response.Items.Add(new CartItemResponseDto
                {
                    ProductoId = product.ProductoID,
                    Nombre = product.Nombre,
                    PrecioUnitario = product.Precio,
                    Cantidad = item.Cantidad,
                    Subtotal = lineSubtotal,
                    ExistenciasDisponibles = product.Existencias
                });
            }

            subtotal = Round(subtotal);
            var automaticDiscount = 0m;
            foreach (var item in response.Items)
            {
                var product = products.Single(product => product.ProductoID == item.ProductoId);
                var applicable = promotions.Where(p =>
                    p.TipoPromocion == "Producto" && p.Productos.Any(pp => pp.ProductoID == product.ProductoID)
                    || p.TipoPromocion == "Categoria" && p.CategoriaID == product.CategoriaID).ToList();

                var lineDiscount = applicable
                    .Select(p => CalculateDiscount(item.Subtotal, p.TipoDescuento, p.ValorDescuento))
                    .DefaultIfEmpty(0m)
                    .Max();

                if (lineDiscount > 0)
                {
                    automaticDiscount += lineDiscount;
                    response.PromocionesAplicadas.AddRange(applicable
                        .Where(p => CalculateDiscount(item.Subtotal, p.TipoDescuento, p.ValorDescuento) == lineDiscount)
                        .Select(p => p.Nombre));
                }
            }

            var seasonalDiscount = promotions
                .Where(p => p.TipoPromocion == "Temporada")
                .Select(p => CalculateDiscount(subtotal, p.TipoDescuento, p.ValorDescuento))
                .DefaultIfEmpty(0m)
                .Max();
            if (seasonalDiscount > 0)
            {
                automaticDiscount += seasonalDiscount;
                response.PromocionesAplicadas.AddRange(promotions
                    .Where(p => p.TipoPromocion == "Temporada"
                        && CalculateDiscount(subtotal, p.TipoDescuento, p.ValorDescuento) == seasonalDiscount)
                    .Select(p => p.Nombre));
            }

            automaticDiscount = Math.Min(Round(automaticDiscount), subtotal);
            var remainingSubtotal = subtotal - automaticDiscount;
            var couponDiscount = 0m;
            if (!string.IsNullOrWhiteSpace(request.CodigoCupon))
            {
                var code = request.CodigoCupon.Trim().ToUpperInvariant();
                var coupon = await promocionRepository.GetPromotionByCouponAsync(code);
                ValidateCoupon(coupon, remainingSubtotal);
                couponDiscount = CalculateDiscount(remainingSubtotal, coupon!.TipoDescuento, coupon.ValorDescuento);
                response.CodigoCupon = coupon.CodigoCupon;
                response.PromocionesAplicadas.Add(coupon.Nombre);
            }

            response.Subtotal = subtotal;
            response.Descuento = Math.Min(Round(automaticDiscount + couponDiscount), subtotal);
            response.Total = Round(subtotal - response.Descuento);
            response.PromocionesAplicadas = response.PromocionesAplicadas.Distinct().ToList();
            return response;
        }

        private static void ValidateCoupon(Promocion? coupon, decimal subtotal)
        {
            if (coupon == null || coupon.TipoPromocion != "Cupon")
                throw new ArgumentException("El cupón no existe");
            if (!coupon.Activo)
                throw new ArgumentException("El cupón no está activo");
            var now = DateTime.Now;
            if (now < coupon.FechaInicio || now.Date > coupon.FechaFin.Date)
                throw new ArgumentException("El cupón no está vigente");
            if (coupon.UsosMaximos.HasValue && coupon.UsosActuales >= coupon.UsosMaximos.Value)
                throw new InvalidOperationException("El cupón ya alcanzó su límite de usos");
            if (coupon.MontoMinimoCompra.HasValue && subtotal < coupon.MontoMinimoCompra.Value)
                throw new ArgumentException($"El cupón requiere una compra mínima de ${coupon.MontoMinimoCompra.Value:0.00}");
        }

        private static decimal CalculateDiscount(decimal amount, string type, decimal value)
        {
            var discount = type == "Porcentaje" ? amount * value / 100m : value;
            return Math.Min(Round(discount), amount);
        }

        private static decimal Round(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
