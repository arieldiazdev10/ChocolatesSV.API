namespace ChocolatesSV.Entities.DTO
{
    public class TopProductoDto
    {
        public int Posicion { get; set; }
        public int IdProducto { get; set; }
        public string Producto { get; set; } = string.Empty;
        public string? ImagenUrl { get; set; }
        public int UnidadesVendidas { get; set; }
        public decimal Ingresos { get; set; }
    }
}
