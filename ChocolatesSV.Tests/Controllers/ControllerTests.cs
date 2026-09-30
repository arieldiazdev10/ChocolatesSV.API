using ChocolatesSV.API.Controllers;
using ChocolatesSV.BL;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;
using ChocolatesSV.Tests.Fakes;
using ChocolatesSV.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace ChocolatesSV.Tests.Controllers
{
    public class PromocionControllerTests
    {
        private readonly FakePromocionRepository _repo = new();
        private readonly PromocionController _controller;

        public PromocionControllerTests()
        {
            var service = new PromocionService(_repo, TestHelpers.CrearMapper());
            _controller = new PromocionController(service);
        }

        [Fact]
        public async Task ValidateCoupon_CuponValido_RetornaOk()
        {
            _repo.Promociones.Add(TestHelpers.Cupon());

            var result = await _controller.ValidateCoupon(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 20 });

            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<ValidarCuponResponseDto>(ok.Value);
            Assert.Equal(2m, body.MontoDescuento);
        }

        [Fact]
        public async Task ValidateCoupon_CuponInexistente_RetornaBadRequest()
        {
            var result = await _controller.ValidateCoupon(
                new ValidarCuponRequestDto { Codigo = "NOEXISTE", Subtotal = 20 });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_PromocionValida_RetornaCreatedAtAction()
        {
            var result = await _controller.Post(TestHelpers.NuevaPromocionDto());

            var created = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(PromocionController.Get), created.ActionName);
            Assert.Single(_repo.Promociones);
        }

        [Fact]
        public async Task Post_ReglaDeNegocioInvalida_RetornaBadRequest()
        {
            var dto = TestHelpers.NuevaPromocionDto("Combo");
            dto.PrecioCombo = 10;

            var result = await _controller.Post(dto);

            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Empty(_repo.Promociones);
        }

        [Fact]
        public async Task Get_IdInexistente_RetornaNotFound()
        {
            var result = await _controller.Get(999);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_IdExistente_RetornaOkYLaElimina()
        {
            _repo.Promociones.Add(TestHelpers.Cupon());

            var result = await _controller.Delete(1);

            Assert.IsType<OkObjectResult>(result);
            Assert.Empty(_repo.Promociones);
        }

        [Fact]
        public async Task Delete_IdInexistente_RetornaNotFound()
        {
            var result = await _controller.Delete(999);

            Assert.IsType<NotFoundObjectResult>(result);
        }
    }

    public class DashboardControllerTests
    {
        private readonly FakeDashboardRepository _repo = new();
        private readonly DashboardController _controller;

        public DashboardControllerTests()
        {
            var service = new DashboardService(_repo, TestHelpers.CrearMapper());
            _controller = new DashboardController(service);
        }

        [Fact]
        public async Task GetSummary_RetornaOkConResumen()
        {
            _repo.Resumen = new ResumenVentas { PedidosMes = 2, IngresosMes = 30 };

            var result = await _controller.GetSummary();

            var ok = Assert.IsType<OkObjectResult>(result);
            var body = Assert.IsType<DashboardSummaryDto>(ok.Value);
            Assert.Equal(15m, body.TicketPromedioMes);
        }

        [Fact]
        public async Task GetTopProducts_TopInvalido_RetornaBadRequest()
        {
            var result = await _controller.GetTopProducts(top: 50);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetTopProducts_ParametrosValidos_RetornaOk()
        {
            var result = await _controller.GetTopProducts();

            Assert.IsType<OkObjectResult>(result);
        }
    }
}
