using ChocolatesSV.BL;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;
using ChocolatesSV.Tests.Fakes;
using ChocolatesSV.Tests.Helpers;
using Xunit;

namespace ChocolatesSV.Tests.Services
{
    public class PromocionServiceTests
    {
        private readonly FakePromocionRepository _repo = new();
        private readonly PromocionService _service;

        public PromocionServiceTests()
        {
            _service = new PromocionService(_repo, TestHelpers.CrearMapper());
        }

        // ---------- Validación de cupones ----------

        [Theory]
        [InlineData(10, 20, 2.00, 18.00)]
        [InlineData(25, 8, 2.00, 6.00)]
        [InlineData(50, 3.50, 1.75, 1.75)]
        public async Task ValidateCoupon_CuponPorcentajeValido_CalculaDescuentoCorrecto(
            decimal porcentaje, decimal subtotal, decimal descuentoEsperado, decimal totalEsperado)
        {
            _repo.Promociones.Add(TestHelpers.Cupon(valor: porcentaje));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = subtotal });

            Assert.True(result.Valido);
            Assert.Equal(descuentoEsperado, result.MontoDescuento);
            Assert.Equal(totalEsperado, result.TotalConDescuento);
        }

        [Fact]
        public async Task ValidateCoupon_CodigoEnMinusculasYConEspacios_LoNormalizaYLoAcepta()
        {
            _repo.Promociones.Add(TestHelpers.Cupon());

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "  dulce10 ", Subtotal = 20 });

            Assert.True(result.Valido);
        }

        [Fact]
        public async Task ValidateCoupon_CodigoInexistente_RetornaInvalido()
        {
            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "NOEXISTE", Subtotal = 20 });

            Assert.False(result.Valido);
            Assert.Equal("El cupón no existe", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_CuponVencido_RetornaExpirado()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(
                inicio: DateTime.Today.AddDays(-30), fin: DateTime.Today.AddDays(-1)));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 20 });

            Assert.False(result.Valido);
            Assert.Equal("El cupón ha expirado", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_CuponQueAunNoInicia_RetornaNoVigente()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(
                inicio: DateTime.Today.AddDays(5), fin: DateTime.Today.AddDays(30)));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 20 });

            Assert.False(result.Valido);
            Assert.Equal("El cupón todavía no está vigente", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_CuponInactivo_RetornaInvalido()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(activo: false));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 20 });

            Assert.False(result.Valido);
            Assert.Equal("El cupón no está activo", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_LimiteDeUsosAlcanzado_RetornaInvalido()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(usosMaximos: 5, usosActuales: 5));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 20 });

            Assert.False(result.Valido);
            Assert.Equal("El cupón ya alcanzó su límite de usos", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_SubtotalMenorAlMinimo_RetornaInvalido()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(minimo: 5));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 3 });

            Assert.False(result.Valido);
            Assert.Contains("compra mínima", result.Mensaje);
        }

        [Fact]
        public async Task ValidateCoupon_MontoFijoMayorAlSubtotal_DescuentoNoSuperaElSubtotal()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(tipoDescuento: "MontoFijo", valor: 10));

            var result = await _service.ValidateCouponAsync(
                new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 6 });

            Assert.True(result.Valido);
            Assert.Equal(6m, result.MontoDescuento);
            Assert.Equal(0m, result.TotalConDescuento);
        }

        // ---------- Listado público ----------

        [Fact]
        public async Task GetActivePromotions_ConCuponesYTemporada_NoExponeCupones()
        {
            _repo.Promociones.Add(TestHelpers.Cupon());
            _repo.Promociones.Add(new Promocion
            {
                PromocionID = 2,
                Nombre = "Navidad",
                TipoPromocion = "Temporada",
                TipoDescuento = "MontoFijo",
                ValorDescuento = 3,
                Activo = true,
                FechaInicio = DateTime.Today,
                FechaFin = DateTime.Today.AddDays(10)
            });

            var result = await _service.GetActivePromotionsAsync();

            Assert.Single(result);
            Assert.Equal("Temporada", result[0].Tipo);
        }

        // ---------- Reglas al crear / editar ----------

        [Fact]
        public async Task InsertPromotion_ComboConUnSoloProducto_LanzaArgumentException()
        {
            var dto = TestHelpers.NuevaPromocionDto("Combo");
            dto.PrecioCombo = 10;
            dto.Productos = [new PromocionProductoDto { IdProducto = 1, Cantidad = 1 }];

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.InsertPromotionAsync(dto));
            Assert.Equal("Un combo necesita al menos 2 productos", ex.Message);
        }

        [Fact]
        public async Task InsertPromotion_PorcentajeMayorA100_LanzaArgumentException()
        {
            var dto = TestHelpers.NuevaPromocionDto();
            dto.Descuento = 150;

            await Assert.ThrowsAsync<ArgumentException>(() => _service.InsertPromotionAsync(dto));
        }

        [Fact]
        public async Task InsertPromotion_FechaFinAnteriorAInicio_LanzaArgumentException()
        {
            var dto = TestHelpers.NuevaPromocionDto();
            dto.FechaFin = dto.FechaInicio.AddDays(-1);

            await Assert.ThrowsAsync<ArgumentException>(() => _service.InsertPromotionAsync(dto));
        }

        [Fact]
        public async Task InsertPromotion_CuponSinCodigo_LanzaArgumentException()
        {
            var dto = TestHelpers.NuevaPromocionDto("Cupon");
            dto.Cupon = null;

            await Assert.ThrowsAsync<ArgumentException>(() => _service.InsertPromotionAsync(dto));
        }

        [Fact]
        public async Task InsertPromotion_CuponDuplicado_LanzaArgumentException()
        {
            _repo.Promociones.Add(TestHelpers.Cupon(codigo: "AMOR15"));
            var dto = TestHelpers.NuevaPromocionDto("Cupon");
            dto.Cupon = "amor15";

            var ex = await Assert.ThrowsAsync<ArgumentException>(() => _service.InsertPromotionAsync(dto));
            Assert.Contains("AMOR15", ex.Message);
        }

        [Fact]
        public async Task InsertPromotion_CuponValido_GuardaCodigoEnMayusculasYAsignaId()
        {
            var dto = TestHelpers.NuevaPromocionDto("Cupon");
            dto.Cupon = "amor15";

            var result = await _service.InsertPromotionAsync(dto);

            Assert.Equal(1, result.Id);
            Assert.Equal("AMOR15", _repo.Promociones.Single().CodigoCupon);
        }

        [Fact]
        public async Task InsertPromotion_TipoTemporadaConCupon_LimpiaCamposQueNoAplican()
        {
            var dto = TestHelpers.NuevaPromocionDto("Temporada");
            dto.Cupon = "NOAPLICA";
            dto.PrecioCombo = 99;

            await _service.InsertPromotionAsync(dto);

            var guardada = _repo.Promociones.Single();
            Assert.Null(guardada.CodigoCupon);
            Assert.Null(guardada.PrecioCombo);
        }

        [Fact]
        public async Task UpdatePromotion_IdInexistente_RetornaNull()
        {
            var result = await _service.UpdatePromotionAsync(999, TestHelpers.NuevaPromocionDto());

            Assert.Null(result);
        }
    }
}