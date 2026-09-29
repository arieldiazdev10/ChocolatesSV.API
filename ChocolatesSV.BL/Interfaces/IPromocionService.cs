using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IPromocionService
    {
        public Task<List<PromocionDto>> GetActivePromotionsAsync();
        public Task<PromocionDto?> GetPromotionByIdAsync(int id);
        public Task<ValidarCuponResponseDto> ValidateCouponAsync(ValidarCuponRequestDto request);
        public Task<PromocionDto> InsertPromotionAsync(PromocionDto promocion);
        public Task<PromocionDto?> UpdatePromotionAsync(int id, PromocionDto promocion);
        public Task<bool> DeletePromotionAsync(int id);
    }
}