using System.Globalization;
using System.Text.RegularExpressions;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL
{
    public class PaymentSimulationService : IPaymentSimulationService
    {
        public Task<SimulatePaymentResponseDto> SimulateAsync(SimulatePaymentRequestDto request)
        {
            var number = Regex.Replace(request.NumeroTarjeta ?? string.Empty, "[ -]", string.Empty);
            var validCard = number.Length is >= 13 and <= 19 && number.All(char.IsDigit);
            var validExpiration = DateTime.TryParseExact(
                $"01/{request.FechaExpiracion}",
                "dd/MM/yy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var expiration)
                && expiration >= new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var validCvv = Regex.IsMatch(request.Cvv ?? string.Empty, "^[0-9]{3,4}$");

            if (!validCard || !validExpiration || !validCvv || request.Monto <= 0)
            {
                return Task.FromResult(new SimulatePaymentResponseDto
                {
                    Aprobado = false,
                    Mensaje = "Los datos del pago no tienen un formato válido",
                    Monto = request.Monto
                });
            }

            return Task.FromResult(new SimulatePaymentResponseDto
            {
                Aprobado = true,
                Mensaje = "Pago simulado aprobado",
                Referencia = $"SIM-{Guid.NewGuid():N}".ToUpperInvariant(),
                Monto = request.Monto
            });
        }
    }
}
