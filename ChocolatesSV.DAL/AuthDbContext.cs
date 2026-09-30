using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ChocolatesSV.Entities.Models;

namespace ChocolatesSV.DAL
{
    public class AuthDbContext : IdentityDbContext<Usuario>
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }
    }
}
