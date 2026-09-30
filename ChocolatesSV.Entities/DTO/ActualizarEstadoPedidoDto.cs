using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class ActualizarEstadoPedidoDto
    {
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;
    }
}