using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public int IdCategoria { get; set; }

        [Required(ErrorMessage ="El nombre del producto es requerido")]
        [StringLength(50, ErrorMessage ="El nombre del producto no puede tener más de 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio del producto es requerido")]
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EsDestacado { get; set; }
        public int Stock { get; set; }
        public bool Activo { get; set; }
    }
}
