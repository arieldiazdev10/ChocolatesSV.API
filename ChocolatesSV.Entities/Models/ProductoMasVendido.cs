namespace ChocolatesSV.Entities.Models
{
    // Resultado de la consulta de más vendidos (no es una tabla)
    public class ProductoMasVendido
    {
        public int ProductoID { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? URLImagen { get; set; }
        public int CantidadVendida { get; set; }
        public decimal TotalVendido { get; set; }
    }
}
