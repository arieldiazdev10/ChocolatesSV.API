using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IDashboardService
    {
        public Task<DashboardSummaryDto> GetSummaryAsync();
    }
}
