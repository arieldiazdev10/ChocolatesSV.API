using AutoMapper;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL
{
    public class CategoriaService(ICategoriaRepository categoriaRepository, IMapper mapper) : ICategoriaService
    {
        public async Task<List<CategoriaDto>> GetAllCategoriesAsync()
        {
            var categorias = await categoriaRepository.GetAllCategoriesAsync();
            return mapper.Map<List<CategoriaDto>>(categorias);
        }
    }
}