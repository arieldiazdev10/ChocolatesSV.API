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
                .ForMember(d => d.NumeroOrden, o => o.MapFrom(s => s.CodigoOrden))
                .ForMember(d => d.Cliente, o => o.MapFrom(s => s.NombreCliente))
                .ForMember(d => d.Correo, o => o.MapFrom(s => s.CorreoCliente))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.TelefonoCliente))
                .ForMember(d => d.Estado, o => o.MapFrom(s => s.EstadoPedido))
                .ForMember(d => d.FechaEntrega, o => o.MapFrom(s => s.FechaEntrega))
                .ForMember(d => d.FechaCreacion, o => o.MapFrom(s => s.FechaCreacion));

            CreateMap<Pedido, PedidoCompletoDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.PedidoID))
                .ForMember(d => d.NumeroOrden, o => o.MapFrom(s => s.CodigoOrden))
                .ForMember(d => d.Cliente, o => o.MapFrom(s => s.NombreCliente))
                .ForMember(d => d.Correo, o => o.MapFrom(s => s.CorreoCliente))
                .ForMember(d => d.Telefono, o => o.MapFrom(s => s.TelefonoCliente))
                .ForMember(d => d.Subtotal, o => o.MapFrom(s => s.SubTotal))
                .ForMember(d => d.Descuento, o => o.MapFrom(s => s.DescuentoAplicado))
                .ForMember(d => d.Estado, o => o.MapFrom(s => s.EstadoPedido))
                .ForMember(d => d.SiguientesEstados, o => o.MapFrom(s => EstadosPedido.ObtenerSiguientes(s.EstadoPedido)))
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.Detalles));

            CreateMap<Pedido, TrackOrderResponseDto>()
                .ForMember(d => d.NumeroOrden, o => o.MapFrom(s => s.CodigoOrden))
                .ForMember(d => d.Estado, o => o.MapFrom(s => s.EstadoPedido))
                .ForMember(d => d.Detalles, o => o.MapFrom(s => s.Detalles));

            CreateMap<PedidoDetalle, PedidoDetalleDto>()
                .ForMember(d => d.ProductoId, o => o.MapFrom(s => s.ProductoID));



            CreateMap<PedidoDto, Pedido>();
        }
    }
}