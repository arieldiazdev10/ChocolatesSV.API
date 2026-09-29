using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class CategoriaRepository(IDatabaseRepository databaseRepository) : ICategoriaRepository
    {
        private static class Queries
        {
            public const string GetAll = "SELECT * FROM Categorias ORDER BY Nombre";
        }

        public async Task<List<Categoria>> GetAllCategoriesAsync()
        {
            return [.. (await databaseRepository.QueryAsync<Categoria>(Queries.GetAll))];
        }
    }
}