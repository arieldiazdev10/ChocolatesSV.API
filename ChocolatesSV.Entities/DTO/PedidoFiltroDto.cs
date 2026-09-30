namespace ChocolatesSV.Entities.DTO
{
    public class PedidoFiltroDto
    {
        public string? Estado { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }
}