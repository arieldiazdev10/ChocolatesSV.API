using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.DTO
{
    public class PromocionProductoDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "El Id del producto es inválido")]
        public int IdProducto { get; set; }

        [Range(1, 100, ErrorMessage = "La cantidad debe estar entre 1 y 100")]
        public int Cantidad { get; set; } = 1;

        public string? NombreProducto { get; set; }
        public decimal? PrecioProducto { get; set; }
    }
}
