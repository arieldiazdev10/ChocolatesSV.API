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
        public async Task<List<PedidoDto>> GetAllOrdersAsync()
        {
            var pedidos = await pedidoRepository.GetAllOrdersAsync();
            return mapper.Map<List<PedidoDto>>(pedidos);
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
