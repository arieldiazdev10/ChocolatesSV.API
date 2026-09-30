using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IPedidoService
    {
        Task<List<PedidoDto>> GetAllOrdersAsync(PedidoFiltroDto filtro);
        Task<PedidoCompletoDto?> GetOrderByIdAsync(int id);
        Task<PedidoCompletoDto> UpdateOrderStatusAsync(int id, ActualizarEstadoPedidoDto request);
        Task<TrackOrderResponseDto> TrackOrderAsync(TrackOrderRequestDto request);
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
    }
}