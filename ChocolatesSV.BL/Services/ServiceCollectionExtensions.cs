using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.BL.Profiles;
using Microsoft.Extensions.DependencyInjection;

namespace ChocolatesSV.BL.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServiceConnector(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<ProductoProfile>();
                cfg.AddProfile<PedidoProfile>();
                cfg.AddProfile<CategoriaProfile>();
                cfg.AddProfile<PromocionProfile>();     // Dev 2
            });

            services.AddTransient<IProductoService, ProductoService>();
            services.AddTransient<ICategoriaService, CategoriaService>();
            services.AddTransient<IPedidoService, PedidoService>();
            services.AddTransient<IPromocionService, PromocionService>();   // Dev 2
            return services;
        }
    }
}