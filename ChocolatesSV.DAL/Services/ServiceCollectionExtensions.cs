using ChocolatesSV.DAL.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace ChocolatesSV.DAL.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRepositoryConnector(this IServiceCollection services)

        {
            services.AddTransient<IDatabaseRepository, DatabaseRepository>();
            services.AddTransient<IProductoRepository, ProductoRepository>();
            services.AddTransient<ICategoriaRepository, CategoriaRepository>();
            services.AddTransient<IPedidoRepository, PedidoRepository>();
            services.AddTransient<IPromocionRepository, PromocionRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IContenidoRepository, ContenidoRepository>();
            return services;
        }
    }
}