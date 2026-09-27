using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class AuthRepository(IDatabaseRepository databaseRepository) : IAuthRepository
    {
        private static class Queries
        {
            public const string Login =
                @"SELECT *
                  FROM Usuarios
                  WHERE Correo = @Correo
                  AND ContrasenaHash = @Password
                  AND Activo = 1";
        }

        public async Task<Usuario?> LoginAsync(
            string correo,
            string password)
        {
            return await databaseRepository.QueryFirstOrDefaultAsync<Usuario>(Queries.Login, new
            {
                        Correo = correo,
                        Password = password
            });
        }
    }
}