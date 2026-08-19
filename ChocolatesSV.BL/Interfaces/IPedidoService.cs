using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.DTO;

namespace ChocolatesSV.BL.Interfaces
{
    public interface IPedidoService
    {
        Task<List<PedidoDto>> GetAllOrdersAsync();
    }
}
