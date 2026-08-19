using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using ChocolatesSV.BL.Interfaces;

namespace ChocolatesSV.BL
{
    public class PedidoService(
        IPedidoRepository pedidoRepository,
        IMapper mapper) : IPedidoService
    {
        public async Task<List<PedidoDto>> GetAllOrdersAsync()
        {
            var pedidos = await pedidoRepository.GetAllOrdersAsync();
            return mapper.Map<List<PedidoDto>>(pedidos);
        }
    }
}
