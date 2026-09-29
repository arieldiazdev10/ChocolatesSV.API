namespace ChocolatesSV.Entities.Models
{
    public class PromocionProducto
    {
        public int PromocionID { get; set; }
        public int ProductoID { get; set; }
        public int Cantidad { get; set; }
        public string? Nombre { get; set; }
        public decimal? Precio { get; set; }
    }
}