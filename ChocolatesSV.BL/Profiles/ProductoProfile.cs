using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
    public class ProductoProfile : Profile
    {
        public ProductoProfile() 
        {
            CreateMap<Producto, ProductoDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductoID))
                .ForMember(dest => dest.IdCategoria, opt => opt.MapFrom(src => src.CategoriaID))
                .ForMember(dest => dest.Nombre, opt => opt.MapFrom(src => src.Nombre))
                .ForMember(dest => dest.Descripcion, opt => opt.MapFrom(src => src.Descripcion))
                .ForMember(dest => dest.Precio, opt => opt.MapFrom(src => src.Precio))
                .ForMember(dest => dest.ImagenUrl, opt => opt.MapFrom(src => src.URLImagen))
                .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Existencias))
                .ForMember(dest => dest.Activo, opt => opt.MapFrom(src => src.Activo))
                .ReverseMap();
        }
    }
}
