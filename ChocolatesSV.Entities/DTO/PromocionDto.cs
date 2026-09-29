using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class PromocionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la promoción es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "El tipo de promoción es requerido")]
        [RegularExpression("^(Temporada|Producto|Categoria|Combo|Cupon)$",
            ErrorMessage = "Tipo inválido. Use: Temporada, Producto, Categoria, Combo o Cupon")]
        public string Tipo { get; set; } = string.Empty;

        [Required(ErrorMessage = "El tipo de descuento es requerido")]
        [RegularExpression("^(Porcentaje|MontoFijo)$",
            ErrorMessage = "Tipo de descuento inválido. Use: Porcentaje o MontoFijo")]
        public string TipoDescuento { get; set; } = string.Empty;

        [Range(0, 10000, ErrorMessage = "El descuento debe estar entre 0 y 10,000")]
        public decimal Descuento { get; set; }

        [Range(0.01, 10000, ErrorMessage = "El precio del combo debe ser mayor a 0")]
        public decimal? PrecioCombo { get; set; }

        [StringLength(30, MinimumLength = 4, ErrorMessage = "El cupón debe tener entre 4 y 30 caracteres")]
        [RegularExpression("^[A-Za-z0-9_-]+$", ErrorMessage = "El cupón solo admite letras, números, - y _")]
        public string? Cupon { get; set; }

        [Range(0, 100000, ErrorMessage = "La compra mínima no puede ser negativa")]
        public decimal? CompraMinima { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El límite de usos debe ser mayor a 0")]
        public int? LimiteUsos { get; set; }

        public int UsosRealizados { get; set; }

        public int? IdCategoria { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        public bool Activa { get; set; } = true;

        public List<PromocionProductoDto> Productos { get; set; } = [];
    }
}
