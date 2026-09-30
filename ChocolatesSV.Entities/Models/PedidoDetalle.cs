using System.ComponentModel.DataAnnotations;

namespace ChocolatesSV.Entities.Models
{
    public class PedidoDetalle
    {
        [Key]
        public int PedidoDetalleID { get; set; }
        public int PedidoID { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
    }
}
