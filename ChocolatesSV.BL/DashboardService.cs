using AutoMapper;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL
{
    public class DashboardService(IDashboardRepository dashboardRepository, IMapper mapper) : IDashboardService
    {
        public async Task<DashboardSummaryDto> GetSummaryAsync()
        {
            var resumen = await dashboardRepository.GetSalesSummaryAsync();
            var estados = await dashboardRepository.GetOrdersByStatusAsync();

            var dto = mapper.Map<DashboardSummaryDto>(resumen);
            dto.PedidosPorEstado = mapper.Map<List<EstadoPedidoDto>>(estados);

            // Regla de negocio: ticket promedio = ingresos del mes / ventas del mes
            dto.TicketPromedioMes = resumen.PedidosMes > 0
                ? Math.Round(resumen.IngresosMes / resumen.PedidosMes, 2)
                : 0;

            return dto;
        }
    }
}
