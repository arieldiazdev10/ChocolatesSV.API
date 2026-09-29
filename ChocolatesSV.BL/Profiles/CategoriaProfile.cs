using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
    public class CategoriaProfile : Profile
    {
        public CategoriaProfile()
        {
            CreateMap<Categoria, CategoriaDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CategoriaID))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion));
        }
    }
}