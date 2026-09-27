using System;
using System.Collections.Generic;
using System.Text;
using ChocolatesSV.BL.Interfaces;
using ChocolatesSV.DAL.Interfaces;
using ChocolatesSV.Entities.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChocolatesSV.BL
{
    public class AuthService(IAuthRepository authRepository, IConfiguration configuration) : IAuthService
    {
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            var usuario = await authRepository.LoginAsync(request.Correo, request.Contrasena);

            if (usuario == null)
            {
                return null;
            }

            var key = configuration["Jwt:Key"];

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, usuario.Correo),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim("UsuarioId", usuario.UsuarioID.ToString())
            };

            var securityKey =
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key!));

            var credentials =
                new SigningCredentials(
                    securityKey,
                    SecurityAlgorithms.HmacSha256);

            var token =
                new JwtSecurityToken(
                    issuer: configuration["Jwt:Issuer"],
                    audience: configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(8),
                    signingCredentials: credentials
                );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };
        }
    }
}