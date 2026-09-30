using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL.Interfaces
{
     public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllOrdersAsync();
        Task<int> InsertOrderAsync(Pedido pedido, IDbTransaction transaction);
        Task InsertDetailsAsync(int pedidoId, IEnumerable<PedidoDetalle> detalles, IDbTransaction transaction);
    }
}
