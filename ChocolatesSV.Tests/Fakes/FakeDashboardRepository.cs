using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.Tests.Fakes
{
    public class FakeDashboardRepository : IDashboardRepository
    {
        public ResumenVentas Resumen { get; set; } = new();
        public List<PedidosPorEstado> Estados { get; set; } = [];
        public List<ProductoMasVendido> TopProductos { get; set; } = [];

        // Guarda los parámetros recibidos para verificarlos en las pruebas
        public (int Top, DateTime? Desde, DateTime? Hasta)? UltimaConsultaTop { get; private set; }

        public Task<ResumenVentas> GetSalesSummaryAsync() => Task.FromResult(Resumen);

        public Task<List<PedidosPorEstado>> GetOrdersByStatusAsync() => Task.FromResult(Estados);

        public Task<List<ProductoMasVendido>> GetTopProductsAsync(int top, DateTime? desde, DateTime? hasta)
        {
            UltimaConsultaTop = (top, desde, hasta);
            return Task.FromResult(TopProductos.Take(top).ToList());
        }
    }
}