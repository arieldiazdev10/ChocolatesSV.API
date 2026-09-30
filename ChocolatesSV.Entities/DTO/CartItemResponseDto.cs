namespace ChocolatesSV.Entities.DTO
{
    public class CartItemResponseDto
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public decimal Subtotal { get; set; }
        public int ExistenciasDisponibles { get; set; }
    }
}
