using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class CreateOrderRequestDto
    {
        [Required(ErrorMessage = "El nombre del cliente es requerido")]
        [StringLength(150)]
        public string NombreCliente { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo del cliente es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(150)]
        public string CorreoCliente { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no tiene un formato válido")]
        [StringLength(30)]
        public string? TelefonoCliente { get; set; }

        [Required(ErrorMessage = "La fecha de entrega es requerida")]
        public DateTime FechaEntrega { get; set; }

        [StringLength(500)]
        public string? Comentarios { get; set; }

        [Required(ErrorMessage = "El carrito es requerido")]
        [MinLength(1, ErrorMessage = "El carrito debe contener al menos un producto")]
        public List<CartItemRequestDto> Items { get; set; } = [];

        [StringLength(30)]
        public string? CodigoCupon { get; set; }

        [Required(ErrorMessage = "Los datos del pago son requeridos")]
        public SimulatePaymentRequestDto Pago { get; set; } = new();
    }
}
