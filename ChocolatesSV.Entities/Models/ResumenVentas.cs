namespace ChocolatesSV.Entities.Models
{
    public class ResumenVentas
    {
        public int PedidosHoy { get; set; }
        public decimal IngresosHoy { get; set; }
        public int PedidosMes { get; set; }
        public decimal IngresosMes { get; set; }
        public decimal IngresosTotales { get; set; }
        public int PedidosPendientes { get; set; }
    }
}
