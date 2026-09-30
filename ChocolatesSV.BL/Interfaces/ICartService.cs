using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface ICartService
    {
        Task<CalculateCartResponseDto> CalculateAsync(CalculateCartRequestDto request);
    }
}
