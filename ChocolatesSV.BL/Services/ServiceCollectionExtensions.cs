using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.BL.Profiles;
using ChocolatesSV.DAL;
using ChocolatesSV.DAL.Interfaces;
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
            });

            services.AddAutoMapper(cfg => cfg.AddProfile<ProductoProfile>());
            services.AddTransient<IProductoService, ProductoService>();
            return services;
        }

    }
}
