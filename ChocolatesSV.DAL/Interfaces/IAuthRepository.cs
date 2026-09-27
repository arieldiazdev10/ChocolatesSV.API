using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL.Interfaces
{
    public interface IAuthRepository
    {
        Task<Usuario?> LoginAsync(string correo, string password);
    }
}