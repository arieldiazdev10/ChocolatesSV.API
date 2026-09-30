using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class SimulatePaymentRequestDto
    {
        [Required(ErrorMessage = "El número de tarjeta es requerido")]
        public string NumeroTarjeta { get; set; } = string.Empty;

        [Required(ErrorMessage = "La fecha de expiración es requerida")]
        [RegularExpression("^(0[1-9]|1[0-2])/[0-9]{2}$", ErrorMessage = "La fecha debe tener el formato MM/yy")]
        public string FechaExpiracion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El CVV es requerido")]
        [RegularExpression("^[0-9]{3,4}$", ErrorMessage = "El CVV debe contener 3 o 4 dígitos")]
        public string Cvv { get; set; } = string.Empty;

        [Range(0.01, 999999.99, ErrorMessage = "El monto debe ser mayor que cero")]
        public decimal Monto { get; set; }
    }
}
