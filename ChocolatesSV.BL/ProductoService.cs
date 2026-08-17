using AutoMapper;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL
{
    public class ProductoService(IProductoRepository productoRepository, IMapper mapper) : IProductoService
    {
        public async Task<List<ProductoDto>> GetAllActiveProductsAsync()
        {
            var productos = await productoRepository.GetAllActiveProductsAsync();
            return mapper.Map<List<ProductoDto>>(productos);
        }

        public async Task<List<ProductoDto>> GetFeaturedProductsAsync()
        {
            var productos = await productoRepository.GetFeaturedProductsAsync();
            return mapper.Map<List<ProductoDto>>(productos);
        }

        public async Task<ProductoDto?> GetProductByIdAsync(int id)
        {
            var producto = await productoRepository.GetProductByIdAsync(id);
            return mapper.Map<ProductoDto?>(producto);
        }

        public async Task<ProductoDto> InsertProductAsync(ProductoDto producto)
        {
            var entity = mapper.Map<Producto>(producto);
            var newId = await productoRepository.InsertProductAsync(entity);
            producto.Id = newId;
            return producto;
        }

        public async Task<ProductoDto?> UpdateProductAsync(int id, ProductoDto producto)
        {
            var entity = mapper.Map<Producto>(producto);
            entity.ProductoID = id;
            var updated = await productoRepository.UpdateProductAsync(entity);
            if (!updated)
            {
                return null;
            }
            producto.Id = id;
            return producto;
        }

        public async Task<bool> SoftDeleteProductAsync(int id)
        {
            return await productoRepository.SoftDeleteProductAsync(id);
        }
    }
}
