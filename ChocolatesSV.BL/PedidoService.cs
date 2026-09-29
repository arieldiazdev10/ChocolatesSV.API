using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.BL.Interfaces;

namespace ChocolatesSV.BL
{
    public class PedidoService(
        IPedidoRepository pedidoRepository,
        IMapper mapper) : IPedidoService
    {
        public async Task<List<PedidoDto>> GetAllOrdersAsync()
        {
            var pedidos = await pedidoRepository.GetAllOrdersAsync();
            return mapper.Map<List<PedidoDto>>(pedidos);
        }

        public async Task<PedidoDto?> GetOrderByIdAsync(int id)
        {
            var pedido = await pedidoRepository.GetOrderByIdAsync(id);

            return mapper.Map<PedidoDto?>(pedido);
        }

        public async Task<bool> UpdateOrderStatusAsync(int pedidoId, string estado)
        {
            string[] estadosValidos =
            [
            "Pendiente",
            "Confirmado",
            "En Preparacion",
            "Enviado",
            "Entregado",
            "Cancelado"
            ];

            if (!estadosValidos.Contains(estado))
            {
                return false;
            }

            return await pedidoRepository
            .UpdateOrderStatusAsync(
            pedidoId,
            estado
            );
        }

        public async Task<TrackingResponseDto?> TrackOrderAsync(int pedidoId, string correo)
        {
            var pedido = await pedidoRepository.TrackOrderAsync(pedidoId, correo);

            if (pedido == null)
            {
                return null;
            }

            return new TrackingResponseDto
            {
                PedidoId = pedido.PedidoID,
                Cliente = pedido.NombreCliente,
                Estado = pedido.EstadoPedido,
                Total = pedido.Total,
                FechaEntrega = pedido.FechaEntrega
            };
        }
    }
}
