namespace ChocolatesSV.Entities.DTO
{
    public class PedidoCompletoDto
    {
        public int Id { get; set; }
        public string NumeroOrden { get; set; } = string.Empty;
        public string Cliente { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public DateTime FechaEntrega { get; set; }
        public string? Comentarios { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<string> SiguientesEstados { get; set; } = [];
        public string MetodoPago { get; set; } = string.Empty;
        public string? ReferenciaPago { get; set; }
        public DateTime FechaCreacion { get; set; }
        public List<PedidoDetalleDto> Detalles { get; set; } = [];
    }
}