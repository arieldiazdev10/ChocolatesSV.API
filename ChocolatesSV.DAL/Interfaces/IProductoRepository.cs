using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IProductoRepository
    {
        public Task<List<Producto>> GetAllActiveProductsAsync();
        public Task<List<Producto>> GetFeaturedProductsAsync();
        public Task<Producto?> GetProductByIdAsync(int id);
        public Task<List<Producto>> GetProductsByIdsAsync(IEnumerable<int> ids, IDbTransaction? transaction = null);
        public Task<bool> DecreaseStockAsync(int id, int quantity, IDbTransaction transaction);
        public Task<int> InsertProductAsync(Producto producto);
        public Task<bool> UpdateProductAsync(Producto producto);
        public Task<bool> SoftDeleteProductAsync(int id);
    }
}
