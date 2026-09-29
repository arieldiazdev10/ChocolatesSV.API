using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface ICategoriaRepository
    {
        public Task<List<Categoria>> GetAllCategoriesAsync();
    }
}