namespace ChocolatesSV.Entities.DTO
{
    public class CalculateCartResponseDto
    {
        public List<CartItemResponseDto> Items { get; set; } = [];
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string? CodigoCupon { get; set; }
        public List<string> PromocionesAplicadas { get; set; } = [];
    }
}
