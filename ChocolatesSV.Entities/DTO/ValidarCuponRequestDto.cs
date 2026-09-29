using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class ValidarCuponRequestDto
    {
        [Required(ErrorMessage = "El código del cupón es requerido")]
        [StringLength(30, ErrorMessage = "El código no puede tener más de 30 caracteres")]
        public string Codigo { get; set; } = string.Empty;

        [Range(0.01, 100000, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal Subtotal { get; set; }
    }
}
