using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface ICategoriaService
    {
        public Task<List<CategoriaDto>> GetAllCategoriesAsync();
    }
}