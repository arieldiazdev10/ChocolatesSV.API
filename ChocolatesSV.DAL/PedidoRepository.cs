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
            public const string GetAll = @"SELECT * FROM Pedidos
                WHERE (@Estado IS NULL OR EstadoPedido = @Estado)
                  AND (@FechaDesde IS NULL OR FechaCreacion >= @FechaDesde)
                  AND (@FechaHasta IS NULL OR FechaCreacion < DATEADD(DAY, 1, @FechaHasta))
                ORDER BY FechaCreacion DESC, PedidoID DESC";
            public const string GetById = "SELECT * FROM Pedidos WHERE PedidoID = @PedidoID";
            public const string GetByCodeAndEmail = "SELECT * FROM Pedidos WHERE CodigoOrden = @CodigoOrden AND CorreoCliente = @CorreoCliente";
            public const string GetDetails = "SELECT * FROM PedidoDetalles WHERE PedidoID = @PedidoID ORDER BY PedidoDetalleID";
            public const string UpdateStatus = @"UPDATE Pedidos
                SET EstadoPedido = @NuevoEstado
                WHERE PedidoID = @PedidoID AND EstadoPedido = @EstadoActual";
            public const string RestoreStock = @"UPDATE p
                SET p.Existencias = p.Existencias + d.Total
                FROM Productos p
                INNER JOIN (SELECT ProductoID, SUM(Cantidad) AS Total
                            FROM PedidoDetalles
                            WHERE PedidoID = @PedidoID
                            GROUP BY ProductoID) d ON d.ProductoID = p.ProductoID";
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

        public async Task<List<Pedido>> GetAllOrdersAsync(string? estado, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return [.. await databaseRepository.QueryAsync<Pedido>(Queries.GetAll, new
            {
                Estado = estado,
                FechaDesde = fechaDesde,
                FechaHasta = fechaHasta
            })];
        }

        public async Task<Pedido?> GetOrderByIdAsync(int id)
        {
            var pedido = await databaseRepository.QueryFirstOrDefaultAsync<Pedido>(Queries.GetById, new { PedidoID = id });
            return await LoadDetailsAsync(pedido);
        }

        public async Task<Pedido?> GetOrderByCodeAndEmailAsync(string codigoOrden, string correo)
        {
            var pedido = await databaseRepository.QueryFirstOrDefaultAsync<Pedido>(Queries.GetByCodeAndEmail, new
            {
                CodigoOrden = codigoOrden,
                CorreoCliente = correo
            });
            return await LoadDetailsAsync(pedido);
        }

        public async Task<bool> UpdateOrderStatusAsync(int id, string estadoActual, string nuevoEstado, IDbTransaction transaction)
        {
            return await databaseRepository.ExecuteAsync(Queries.UpdateStatus, new
            {
                PedidoID = id,
                EstadoActual = estadoActual,
                NuevoEstado = nuevoEstado
            }, transaction) > 0;
        }

        public async Task RestoreStockAsync(int pedidoId, IDbTransaction transaction)
        {
            await databaseRepository.ExecuteAsync(Queries.RestoreStock, new { PedidoID = pedidoId }, transaction);
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

        private async Task<Pedido?> LoadDetailsAsync(Pedido? pedido)
        {
            if (pedido is null)
            {
                return null;
            }

            pedido.Detalles = [.. await databaseRepository.QueryAsync<PedidoDetalle>(Queries.GetDetails, new { pedido.PedidoID })];
            return pedido;
        }
    }
}