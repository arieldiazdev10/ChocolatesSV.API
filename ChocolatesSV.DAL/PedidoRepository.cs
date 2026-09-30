using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;
using System.Data;

namespace ChocolatesSV.DAL
{
    public class PedidoRepository(IDatabaseRepository databaseRepository) : IPedidoRepository
    {
        private static class Queries
        {
            public const string GetAll = "SELECT * FROM Pedidos";
            public const string Insert = @"INSERT INTO Pedidos
                (CodigoOrden, NombreCliente, CorreoCliente, TelefonoCliente, FechaEntrega, Comentarios,
                 SubTotal, DescuentoAplicado, Total, EstadoPedido, MetodoPago, ReferenciaPago, FechaCreacion)
                VALUES (@CodigoOrden, @NombreCliente, @CorreoCliente, @TelefonoCliente, @FechaEntrega, @Comentarios,
                        @SubTotal, @DescuentoAplicado, @Total, @EstadoPedido, @MetodoPago, @ReferenciaPago, @FechaCreacion);
                SELECT CAST(SCOPE_IDENTITY() AS int);";
            public const string InsertDetail = @"INSERT INTO PedidoDetalles
                (PedidoID, ProductoID, NombreProducto, PrecioUnitario, Cantidad, Subtotal)
                VALUES (@PedidoID, @ProductoID, @NombreProducto, @PrecioUnitario, @Cantidad, @Subtotal);";
        }

        public async Task<List<Pedido>> GetAllOrdersAsync()
        {
            return [.. await databaseRepository.QueryAsync<Pedido>(Queries.GetAll)];
        }

        public async Task<int> InsertOrderAsync(Pedido pedido, IDbTransaction transaction)
        {
            return await databaseRepository.ExecuteScalarAsync<int>(Queries.Insert, new
            {
                pedido.CodigoOrden,
                pedido.NombreCliente,
                pedido.CorreoCliente,
                pedido.TelefonoCliente,
                pedido.FechaEntrega,
                pedido.Comentarios,
                pedido.SubTotal,
                pedido.DescuentoAplicado,
                pedido.Total,
                pedido.EstadoPedido,
                pedido.MetodoPago,
                pedido.ReferenciaPago,
                pedido.FechaCreacion
            }, transaction);
        }

        public async Task InsertDetailsAsync(int pedidoId, IEnumerable<PedidoDetalle> detalles, IDbTransaction transaction)
        {
            foreach (var detalle in detalles)
            {
                await databaseRepository.ExecuteAsync(Queries.InsertDetail, new
                {
                    PedidoID = pedidoId,
                    detalle.ProductoID,
                    detalle.NombreProducto,
                    detalle.PrecioUnitario,
                    detalle.Cantidad,
                    detalle.Subtotal
                }, transaction);
            }
        }
    }
}
