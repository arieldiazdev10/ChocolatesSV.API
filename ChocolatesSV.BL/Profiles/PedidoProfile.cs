using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL.Profiles
{
   public class PedidoProfile : Profile
    {
        public PedidoProfile()
        {
            CreateMap<Pedido, PedidoDto>()

                .ForMember(d => d.Id, o => o.MapFrom(s => s.PedidoID))
                .ForMember(d => d.Cliente, o => o.MapFrom(s => s.NombreCliente))
                .ForMember(d => d.Correo, o => o.MapFrom(s => s.CorreoCliente))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.TelefonoCliente))
                .ForMember(d => d.Estado, o => o.MapFrom(s => s.EstadoPedido))
                .ForMember(d => d.FechaEntrega, o => o.MapFrom(s => s.FechaEntrega));



            CreateMap<PedidoDto, Pedido>();
        }
    }
}
