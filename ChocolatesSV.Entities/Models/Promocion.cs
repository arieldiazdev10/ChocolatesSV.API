using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class Promocion
    {
        [Key]
        public int PromocionID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string TipoPromocion { get; set; } = string.Empty;
        public string TipoDescuento { get; set; } = string.Empty;
        public decimal ValorDescuento { get; set; }
        public decimal? PrecioCombo { get; set; }
        public string? CodigoCupon { get; set; }
        public decimal? MontoMinimoCompra { get; set; }
        public int? UsosMaximos { get; set; }
        public int UsosActuales { get; set; }
        public int? CategoriaID { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }

        // No es columna: se llena desde la tabla PromocionProductos
        public List<PromocionProducto> Productos { get; set; } = [];
    }
}
