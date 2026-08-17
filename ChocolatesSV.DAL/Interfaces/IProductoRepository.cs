using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IProductoRepository
    {
        public Task<List<Producto>> GetAllActiveProductsAsync();
        public Task<List<Producto>> GetFeaturedProductsAsync();
        public Task<Producto?> GetProductByIdAsync(int id);
        public Task<int> InsertProductAsync(Producto producto);
        public Task<bool> UpdateProductAsync(Producto producto);
        public Task<bool> SoftDeleteProductAsync(int id);
    }
}
