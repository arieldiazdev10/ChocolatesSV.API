using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.BL
{
    public class PedidoService(
        IPedidoRepository pedidoRepository,
        IProductoRepository productoRepository,
        IPromocionRepository promocionRepository,
        IDatabaseRepository databaseRepository,
        ICartService cartService,
        IPaymentSimulationService paymentSimulationService,
        IMapper mapper) : IPedidoService
    {
        public async Task<List<PedidoDto>> GetAllOrdersAsync(PedidoFiltroDto filtro)
        {
            string? estado = null;
            if (!string.IsNullOrWhiteSpace(filtro.Estado))
            {
                estado = EstadosPedido.Normalizar(filtro.Estado)
                    ?? throw new ArgumentException($"Estado inválido. Valores permitidos: {string.Join(", ", EstadosPedido.Todos)}");
            }

            if (filtro.FechaDesde.HasValue && filtro.FechaHasta.HasValue && filtro.FechaDesde.Value.Date > filtro.FechaHasta.Value.Date)
                throw new ArgumentException("La fecha inicial no puede ser mayor que la fecha final");

            var pedidos = await pedidoRepository.GetAllOrdersAsync(estado, filtro.FechaDesde?.Date, filtro.FechaHasta?.Date);
            return mapper.Map<List<PedidoDto>>(pedidos);
        }

        public async Task<PedidoCompletoDto?> GetOrderByIdAsync(int id)
        {
            var pedido = await pedidoRepository.GetOrderByIdAsync(id);
            return pedido is null ? null : mapper.Map<PedidoCompletoDto>(pedido);
        }

        public async Task<PedidoCompletoDto> UpdateOrderStatusAsync(int id, ActualizarEstadoPedidoDto request)
        {
            var nuevoEstado = EstadosPedido.Normalizar(request.Estado)
                ?? throw new ArgumentException($"Estado inválido. Valores permitidos: {string.Join(", ", EstadosPedido.Todos)}");

            var pedido = await pedidoRepository.GetOrderByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido no encontrado");

            if (pedido.EstadoPedido == nuevoEstado)
                throw new InvalidOperationException($"El pedido ya se encuentra en estado {nuevoEstado}");

            if (!EstadosPedido.EsTransicionValida(pedido.EstadoPedido, nuevoEstado))
                throw new InvalidOperationException($"No se puede cambiar el pedido de {pedido.EstadoPedido} a {nuevoEstado}");

            using var transaction = await databaseRepository.BeginTransactionAsync();
            using var connection = transaction.Connection;
            try
            {
                if (!await pedidoRepository.UpdateOrderStatusAsync(id, pedido.EstadoPedido, nuevoEstado, transaction))
                    throw new InvalidOperationException("El estado del pedido cambió mientras se procesaba la solicitud. Intenta de nuevo");

                if (nuevoEstado == EstadosPedido.Cancelado)
                    await pedidoRepository.RestoreStockAsync(id, transaction);

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

            var actualizado = await pedidoRepository.GetOrderByIdAsync(id)
                ?? throw new KeyNotFoundException("Pedido no encontrado");
            return mapper.Map<PedidoCompletoDto>(actualizado);
        }

        public async Task<TrackOrderResponseDto> TrackOrderAsync(TrackOrderRequestDto request)
        {
            var pedido = await pedidoRepository.GetOrderByCodeAndEmailAsync(
                    request.NumeroOrden.Trim().ToUpperInvariant(),
                    request.Correo.Trim())
                ?? throw new KeyNotFoundException("No se encontró un pedido con el número de orden y el correo proporcionados");

            return mapper.Map<TrackOrderResponseDto>(pedido);
        }

        public async Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderRequestDto request)
        {
            var cart = await cartService.CalculateAsync(new CalculateCartRequestDto
            {
                Items = request.Items,
                CodigoCupon = request.CodigoCupon
            });

            var paymentRequest = new SimulatePaymentRequestDto
            {
                NumeroTarjeta = request.Pago.NumeroTarjeta,
                FechaExpiracion = request.Pago.FechaExpiracion,
                Cvv = request.Pago.Cvv,
                Monto = cart.Total
            };
            var payment = await paymentSimulationService.SimulateAsync(paymentRequest);
            if (!payment.Aprobado)
                throw new ArgumentException(payment.Mensaje);

            var codigoOrden = $"ORD-{Guid.NewGuid():N}"[..24].ToUpperInvariant();
            var pedido = new Pedido
            {
                CodigoOrden = codigoOrden,
                NombreCliente = request.NombreCliente.Trim(),
                CorreoCliente = request.CorreoCliente.Trim(),
                TelefonoCliente = request.TelefonoCliente?.Trim() ?? string.Empty,
                FechaEntrega = request.FechaEntrega,
                Comentarios = request.Comentarios?.Trim(),
                SubTotal = cart.Subtotal,
                DescuentoAplicado = cart.Descuento,
                Total = cart.Total,
                EstadoPedido = "Pendiente",
                MetodoPago = "Simulado",
                ReferenciaPago = payment.Referencia,
                FechaCreacion = DateTime.UtcNow
            };

            var details = cart.Items.Select(item => new PedidoDetalle
            {
                ProductoID = item.ProductoId,
                NombreProducto = item.Nombre,
                PrecioUnitario = item.PrecioUnitario,
                Cantidad = item.Cantidad,
                Subtotal = item.Subtotal
            }).ToList();

            using var transaction = await databaseRepository.BeginTransactionAsync();
            try
            {
                foreach (var detail in details)
                {
                    if (!await productoRepository.DecreaseStockAsync(detail.ProductoID, detail.Cantidad, transaction))
                        throw new InvalidOperationException($"Stock insuficiente para el producto {detail.NombreProducto}");
                }

                if (!string.IsNullOrWhiteSpace(request.CodigoCupon)
                    && !await promocionRepository.IncrementCouponUsageAsync(request.CodigoCupon.Trim().ToUpperInvariant(), transaction))
                    throw new InvalidOperationException("El cupón ya no está disponible");

                var orderId = await pedidoRepository.InsertOrderAsync(pedido, transaction);
                await pedidoRepository.InsertDetailsAsync(orderId, details, transaction);
                transaction.Commit();

                return new CreateOrderResponseDto
                {
                    Id = orderId,
                    NumeroOrden = codigoOrden,
                    Estado = pedido.EstadoPedido,
                    Subtotal = pedido.SubTotal,
                    Descuento = pedido.DescuentoAplicado,
                    Total = pedido.Total,
                    ReferenciaPago = pedido.ReferenciaPago ?? string.Empty,
                    FechaCreacion = pedido.FechaCreacion
                };
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}