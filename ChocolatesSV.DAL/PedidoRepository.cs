using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class PedidoRepository(IDatabaseRepository databaseRepository) : IPedidoRepository
    {
        private static class Queries
        {
            public const string GetAll = "SELECT * FROM Pedidos";
            public const string GetById = "SELECT * FROM Pedidos WHERE PedidoID = @PedidoID";
            public const string UpdateStatus =
                @"UPDATE Pedidos
                SET EstadoPedido = @EstadoPedido
                WHERE PedidoID = @PedidoID";

            public const string TrackOrder =
                @"SELECT *
                 FROM Pedidos
                 WHERE PedidoID = @PedidoID
                 AND CorreoCliente = @CorreoCliente";
        }

        public async Task<List<Pedido>> GetAllOrdersAsync()
        {
            return [.. await databaseRepository.QueryAsync<Pedido>(Queries.GetAll)];
        }
        public async Task<Pedido?> GetOrderByIdAsync(int id)
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<Pedido>(Queries.GetById, new { PedidoID = id });

        }
        public async Task<bool> UpdateOrderStatusAsync(int pedidoId, string estado)
        {
            var rowsAffected =
            await databaseRepository.ExecuteAsync(
            Queries.UpdateStatus,
            new
            {
                PedidoID = pedidoId,
                EstadoPedido = estado
            });

            return rowsAffected > 0;
        }

        public async Task<Pedido?> TrackOrderAsync(int pedidoId, string correo)
        {
            return await databaseRepository
            .QueryFirstOrDefaultAsync<Pedido>(
            Queries.TrackOrder,
            new
            {
                PedidoID = pedidoId,
                CorreoCliente = correo
            });
        }
    }
}
