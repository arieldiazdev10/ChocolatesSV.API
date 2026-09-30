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

        public async Task<List<TopProductoDto>> GetTopProductsAsync(int top, DateTime? desde, DateTime? hasta)
        {
            // Reglas de negocio
            if (top < 1 || top > 20)
                throw new ArgumentException("El parámetro top debe estar entre 1 y 20");

            if (desde.HasValue && hasta.HasValue && hasta < desde)
                throw new ArgumentException("La fecha 'hasta' no puede ser anterior a 'desde'");

            var productos = await dashboardRepository.GetTopProductsAsync(top, desde, hasta);
            var resultado = mapper.Map<List<TopProductoDto>>(productos);

            // Ranking: 1, 2, 3...
            for (int i = 0; i < resultado.Count; i++)
            {
                resultado[i].Posicion = i + 1;
            }

            return resultado;
        }
    }
}
