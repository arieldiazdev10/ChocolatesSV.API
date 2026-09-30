namespace ChocolatesSV.Entities.DTO
{
    public class TrackOrderResponseDto
    {
        public string NumeroOrden { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaEntrega { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalleDto> Detalles { get; set; } = [];
    }
}