using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class ProductoDto
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "La categoría del producto es requerida")]
        public int IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(50, ErrorMessage = "El nombre del producto no puede tener más de 50 caracteres")]
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio del producto es requerido")]
        [Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor que cero")]
        public decimal Precio { get; set; }
        public string? ImagenUrl { get; set; }
        public bool EsDestacado { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo")]
        public int Stock { get; set; }
        public bool Activo { get; set; }
    }
}
