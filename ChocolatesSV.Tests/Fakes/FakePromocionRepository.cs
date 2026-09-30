using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.Tests.Fakes
{
    // Repositorio falso en memoria: evita depender de SQL Server en las pruebas
    public class FakePromocionRepository : IPromocionRepository
    {
        public List<Promocion> Promociones { get; } = [];

        public Task<List<Promocion>> GetActivePromotionsAsync() =>
            Task.FromResult(Promociones.Where(p => p.Activo).ToList());

        public Task<List<Promocion>> GetAllPromotionsAsync() =>
            Task.FromResult(Promociones.ToList());

        public Task<Promocion?> GetPromotionByIdAsync(int id) =>
            Task.FromResult(Promociones.FirstOrDefault(p => p.PromocionID == id));

        public Task<Promocion?> GetPromotionByCouponAsync(string codigo, IDbTransaction? transaction = null) =>
            Task.FromResult(Promociones.FirstOrDefault(p => p.CodigoCupon == codigo));

        public Task<bool> IncrementCouponUsageAsync(string codigo, IDbTransaction transaction) =>
            Task.FromResult(true);

        public Task<bool> CouponExistsAsync(string codigo, int? excluirId = null) =>
            Task.FromResult(Promociones.Any(p => p.CodigoCupon == codigo && p.PromocionID != excluirId));

        public Task<int> InsertPromotionAsync(Promocion promocion)
        {
            promocion.PromocionID = Promociones.Count == 0 ? 1 : Promociones.Max(p => p.PromocionID) + 1;
            Promociones.Add(promocion);
            return Task.FromResult(promocion.PromocionID);
        }

        public Task<bool> UpdatePromotionAsync(Promocion promocion)
        {
            var index = Promociones.FindIndex(p => p.PromocionID == promocion.PromocionID);
            if (index < 0) return Task.FromResult(false);
            Promociones[index] = promocion;
            return Task.FromResult(true);
        }

        public Task<bool> DeletePromotionAsync(int id) =>
            Task.FromResult(Promociones.RemoveAll(p => p.PromocionID == id) > 0);
    }
}
