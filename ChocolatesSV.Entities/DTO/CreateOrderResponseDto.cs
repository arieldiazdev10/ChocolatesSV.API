namespace ChocolatesSV.Entities.DTO
{
    public class CreateOrderResponseDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string ReferenciaPago { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
