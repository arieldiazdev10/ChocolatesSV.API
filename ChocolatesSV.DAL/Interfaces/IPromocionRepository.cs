using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IPromocionRepository
    {
        public Task<List<Promocion>> GetActivePromotionsAsync();
        public Task<Promocion?> GetPromotionByIdAsync(int id);
        public Task<Promocion?> GetPromotionByCouponAsync(string codigo, IDbTransaction? transaction = null);
        public Task<bool> IncrementCouponUsageAsync(string codigo, IDbTransaction transaction);
        public Task<bool> CouponExistsAsync(string codigo, int? excluirId = null);
        public Task<int> InsertPromotionAsync(Promocion promocion);
        public Task<bool> UpdatePromotionAsync(Promocion promocion);
        public Task<bool> DeletePromotionAsync(int id);
    }
}
