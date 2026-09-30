using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IPedidoService
    {
        Task<List<PedidoDto>> GetAllOrdersAsync();
        Task<PedidoDto?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int pedidoId, string estado);
        Task<TrackingResponseDto?> TrackOrderAsync(int pedidoId, string correo);
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request);
    }
}
