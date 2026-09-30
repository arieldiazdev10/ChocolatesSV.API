using AutoMapper;
using ChocolatesSV.BL.Profiles;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ChocolatesSV.Tests.Helpers
{
    public static class TestHelpers
    {
        // Mismo registro de AutoMapper que usa la API (ServiceCollectionExtensions)
        public static IMapper CrearMapper()
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<PromocionProfile>();
                cfg.AddProfile<DashboardProfile>();
            });
            return services.BuildServiceProvider().GetRequiredService<IMapper>();
        }

        public static Promocion Cupon(
            string codigo = "DULCE10",
            string tipoDescuento = "Porcentaje",
            decimal valor = 10,
            decimal? minimo = null,
            int? usosMaximos = null,
            int usosActuales = 0,
            bool activo = true,
            DateTime? inicio = null,
            DateTime? fin = null) => new()
            {
                PromocionID = 1,
                Nombre = "Cupón de prueba",
                TipoPromocion = "Cupon",
                TipoDescuento = tipoDescuento,
                ValorDescuento = valor,
                CodigoCupon = codigo,
                MontoMinimoCompra = minimo,
                UsosMaximos = usosMaximos,
                UsosActuales = usosActuales,
                Activo = activo,
                FechaInicio = inicio ?? DateTime.Today.AddDays(-1),
                FechaFin = fin ?? DateTime.Today.AddDays(30)
            };

        public static PromocionDto NuevaPromocionDto(string tipo = "Temporada") => new()
        {
            Nombre = "Promoción de prueba",
            Tipo = tipo,
            TipoDescuento = "Porcentaje",
            Descuento = 10,
            FechaInicio = DateTime.Today,
            FechaFin = DateTime.Today.AddDays(10),
            Activa = true
        };
    }
}
