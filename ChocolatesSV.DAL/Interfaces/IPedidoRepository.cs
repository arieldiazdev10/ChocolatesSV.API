using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllOrdersAsync(string? estado, DateTime? fechaDesde, DateTime? fechaHasta);
        Task<Pedido?> GetOrderByIdAsync(int id);
        Task<Pedido?> GetOrderByCodeAndEmailAsync(string codigoOrden, string correo);
        Task<bool> UpdateOrderStatusAsync(int id, string estadoActual, string nuevoEstado, IDbTransaction transaction);
        Task RestoreStockAsync(int pedidoId, IDbTransaction transaction);
        Task<int> InsertOrderAsync(Pedido pedido, IDbTransaction transaction);
        Task InsertDetailsAsync(int pedidoId, IEnumerable<PedidoDetalle> detalles, IDbTransaction transaction);
    }
}