using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IDashboardService
    {
        public Task<DashboardSummaryDto> GetSummaryAsync();

        public Task<List<TopProductoDto>> GetTopProductsAsync(int top, DateTime? desde, DateTime? hasta);
    }
}
