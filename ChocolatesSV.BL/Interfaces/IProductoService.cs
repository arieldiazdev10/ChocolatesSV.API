using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IProductoService
    {
        public Task<List<ProductoDto>> GetAllActiveProductsAsync();
        public Task<List<ProductoDto>> GetFeaturedProductsAsync();
        public Task<ProductoDto?> GetProductByIdAsync(int id);
        public Task<ProductoDto> InsertProductAsync(ProductoDto producto);
        public Task<ProductoDto?> UpdateProductAsync(int id, ProductoDto producto);
        public Task<bool> SoftDeleteProductAsync(int id);

    }
}
