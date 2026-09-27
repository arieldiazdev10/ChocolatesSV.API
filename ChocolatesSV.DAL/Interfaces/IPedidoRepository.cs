using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
     public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllOrdersAsync();

        Task<Pedido?> GetOrderByIdAsync(int id);
        Task<bool> UpdateOrderStatusAsync(int pedidoId, string estado);
        Task<Pedido?> TrackOrderAsync(int pedidoId, string correo);

    }
}
