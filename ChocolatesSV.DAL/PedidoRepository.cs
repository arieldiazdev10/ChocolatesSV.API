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
        }

        public async Task<List<Pedido>> GetAllOrdersAsync()
        {
            return [.. await databaseRepository.QueryAsync<Pedido>(Queries.GetAll)];
        }
    }
}
