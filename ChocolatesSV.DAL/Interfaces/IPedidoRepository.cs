using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
     public interface IPedidoRepository
    {
        Task<List<Pedido>> GetAllOrdersAsync();
    }
}
