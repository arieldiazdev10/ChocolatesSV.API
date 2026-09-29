using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IDashboardRepository
    {
        public Task<ResumenVentas> GetSalesSummaryAsync();
        public Task<List<PedidosPorEstado>> GetOrdersByStatusAsync();
    }
}
