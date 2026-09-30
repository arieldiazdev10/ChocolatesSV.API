using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class TrackOrderRequestDto
    {
        [Required(ErrorMessage = "El número de orden es requerido")]
        [StringLength(32)]
        public string NumeroOrden { get; set; } = string.Empty;

        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no tiene un formato válido")]
        [StringLength(150)]
        public string Correo { get; set; } = string.Empty;
    }
}