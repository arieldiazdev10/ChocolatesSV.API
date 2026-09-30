using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class CalculateCartRequestDto
    {
        [Required(ErrorMessage = "El carrito es requerido")]
        [MinLength(1, ErrorMessage = "El carrito debe contener al menos un producto")]
        public List<CartItemRequestDto> Items { get; set; } = [];

        [StringLength(30, ErrorMessage = "El cupón no puede tener más de 30 caracteres")]
        public string? CodigoCupon { get; set; }
    }
}
