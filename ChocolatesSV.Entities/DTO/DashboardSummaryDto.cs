namespace ChocolatesSV.Entities.DTO
{
    public class DashboardSummaryDto
    {
        public int VentasDelDia { get; set; }
        public decimal IngresosDelDia { get; set; }
        public int VentasDelMes { get; set; }
        public decimal IngresosDelMes { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal TicketPromedioMes { get; set; }
        public int PedidosPendientes { get; set; }
        public List<EstadoPedidoDto> PedidosPorEstado { get; set; } = [];
    }
}