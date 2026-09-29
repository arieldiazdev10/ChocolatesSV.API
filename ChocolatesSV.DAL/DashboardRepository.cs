using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class DashboardRepository(IDatabaseRepository databaseRepository) : IDashboardRepository
    {
        private static class Queries
        {
            // Una sola consulta calcula todos los indicadores (agregación condicional)
            public const string SalesSummary = @"
                DECLARE @Hoy DATE = CAST(GETDATE() AS DATE);
                DECLARE @InicioMes DATE = DATEFROMPARTS(YEAR(@Hoy), MONTH(@Hoy), 1);

                SELECT
                    COUNT(CASE WHEN CAST(FechaCreacion AS DATE) = @Hoy THEN 1 END)          AS PedidosHoy,
                    ISNULL(SUM(CASE WHEN CAST(FechaCreacion AS DATE) = @Hoy THEN Total END), 0) AS IngresosHoy,
                    COUNT(CASE WHEN FechaCreacion >= @InicioMes THEN 1 END)                  AS PedidosMes,
                    ISNULL(SUM(CASE WHEN FechaCreacion >= @InicioMes THEN Total END), 0)     AS IngresosMes,
                    ISNULL(SUM(Total), 0)                                                    AS IngresosTotales,
                    COUNT(CASE WHEN EstadoPedido = 'Pendiente' THEN 1 END)                   AS PedidosPendientes
                FROM Pedidos
                WHERE EstadoPedido <> 'Cancelado';";

            public const string OrdersByStatus = @"SELECT EstadoPedido, COUNT(*) AS Cantidad
                                                   FROM Pedidos
                                                   GROUP BY EstadoPedido
                                                   ORDER BY Cantidad DESC";
        }

        public async Task<ResumenVentas> GetSalesSummaryAsync()
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<ResumenVentas>(Queries.SalesSummary)
                   ?? new ResumenVentas();
        }

        public async Task<List<PedidosPorEstado>> GetOrdersByStatusAsync()
        {
            return [.. await databaseRepository.QueryAsync<PedidosPorEstado>(Queries.OrdersByStatus)];
        }
    }
}