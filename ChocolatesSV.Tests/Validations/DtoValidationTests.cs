using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Tests.Helpers;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace ChocolatesSV.Tests.Validations
{
    public class DtoValidationTests
    {
        private static List<ValidationResult> Validar(object modelo)
        {
            var resultados = new List<ValidationResult>();
            Validator.TryValidateObject(modelo, new ValidationContext(modelo), resultados, validateAllProperties: true);
            return resultados;
        }

        [Fact]
        public void PromocionDto_DatosCorrectos_PasaValidacion()
        {
            var errores = Validar(TestHelpers.NuevaPromocionDto());

            Assert.Empty(errores);
        }

        [Fact]
        public void PromocionDto_SinNombre_FallaValidacion()
        {
            var dto = TestHelpers.NuevaPromocionDto();
            dto.Nombre = "";

            var errores = Validar(dto);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(PromocionDto.Nombre)));
        }

        [Theory]
        [InlineData("Otro")]
        [InlineData("cupon")]
        [InlineData("DescuentoCategoria")]
        public void PromocionDto_TipoNoPermitido_FallaValidacion(string tipo)
        {
            var dto = TestHelpers.NuevaPromocionDto(tipo);

            var errores = Validar(dto);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(PromocionDto.Tipo)));
        }

        [Fact]
        public void PromocionDto_CuponConCaracteresInvalidos_FallaValidacion()
        {
            var dto = TestHelpers.NuevaPromocionDto("Cupon");
            dto.Cupon = "HOLA 10%";

            var errores = Validar(dto);

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(PromocionDto.Cupon)));
        }

        [Fact]
        public void ValidarCuponRequestDto_SubtotalCero_FallaValidacion()
        {
            var errores = Validar(new ValidarCuponRequestDto { Codigo = "DULCE10", Subtotal = 0 });

            Assert.Contains(errores, e => e.MemberNames.Contains(nameof(ValidarCuponRequestDto.Subtotal)));
        }
    }
}