using ChocolatesSV.BL;
using ChocolatesSV.Entities.Models;
using ChocolatesSV.Tests.Fakes;
using ChocolatesSV.Tests.Helpers;
using Xunit;

namespace ChocolatesSV.Tests.Services
{
    public class DashboardServiceTests
    {
        private readonly FakeDashboardRepository _repo = new();
        private readonly DashboardService _service;

        public DashboardServiceTests()
        {
            _service = new DashboardService(_repo, TestHelpers.CrearMapper());
        }

        [Fact]
        public async Task GetSummary_ConVentasDelMes_CalculaTicketPromedio()
        {
            _repo.Resumen = new ResumenVentas
            {
                PedidosHoy = 2,
                IngresosHoy = 30,
                PedidosMes = 3,
                IngresosMes = 60,
                IngresosTotales = 110,
                PedidosPendientes = 1
            };

            var result = await _service.GetSummaryAsync();

            Assert.Equal(2, result.VentasDelDia);
            Assert.Equal(30m, result.IngresosDelDia);
            Assert.Equal(3, result.VentasDelMes);
            Assert.Equal(60m, result.IngresosDelMes);
            Assert.Equal(110m, result.IngresosTotales);
            Assert.Equal(1, result.PedidosPendientes);
            Assert.Equal(20m, result.TicketPromedioMes);
        }

        [Fact]
        public async Task GetSummary_SinPedidosEnElMes_TicketPromedioEsCero()
        {
            _repo.Resumen = new ResumenVentas();

            var result = await _service.GetSummaryAsync();

            Assert.Equal(0m, result.TicketPromedioMes);
        }

        [Fact]
        public async Task GetSummary_ConPedidosPorEstado_LosMapeaAlDto()
        {
            _repo.Estados =
            [
                new PedidosPorEstado { EstadoPedido = "Entregado", Cantidad = 2 },
                new PedidosPorEstado { EstadoPedido = "Pendiente", Cantidad = 1 }
            ];

            var result = await _service.GetSummaryAsync();

            Assert.Equal(2, result.PedidosPorEstado.Count);
            Assert.Equal("Entregado", result.PedidosPorEstado[0].Estado);
            Assert.Equal(2, result.PedidosPorEstado[0].Cantidad);
        }

        [Fact]
        public async Task GetTopProducts_ConVentas_AsignaPosicionesEnOrden()
        {
            _repo.TopProductos =
            [
                new ProductoMasVendido { ProductoID = 5, Nombre = "Trufa", CantidadVendida = 8, TotalVendido = 20 },
                new ProductoMasVendido { ProductoID = 7, Nombre = "Bombones", CantidadVendida = 2, TotalVendido = 16 }
            ];

            var result = await _service.GetTopProductsAsync(5, null, null);

            Assert.Equal(2, result.Count);
            Assert.Equal(1, result[0].Posicion);
            Assert.Equal("Trufa", result[0].Producto);
            Assert.Equal(8, result[0].UnidadesVendidas);
            Assert.Equal(2, result[1].Posicion);
            Assert.Equal(7, result[1].IdProducto);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(21)]
        [InlineData(-5)]
        public async Task GetTopProducts_TopFueraDeRango_LanzaArgumentException(int top)
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.GetTopProductsAsync(top, null, null));
        }

        [Fact]
        public async Task GetTopProducts_HastaAnteriorADesde_LanzaArgumentException()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.GetTopProductsAsync(5, new DateTime(2026, 9, 30), new DateTime(2026, 9, 1)));
        }

        [Fact]
        public async Task GetTopProducts_ParametrosValidos_LosEnviaAlRepositorio()
        {
            var desde = new DateTime(2026, 9, 1);
            var hasta = new DateTime(2026, 9, 30);

            await _service.GetTopProductsAsync(3, desde, hasta);

            Assert.Equal((3, (DateTime?)desde, (DateTime?)hasta), _repo.UltimaConsultaTop);
        }
    }
}
