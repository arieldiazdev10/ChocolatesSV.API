using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChocolatesSV.Entities.Models;
using ChocolatesSV.DAL;
using Scalar.AspNetCore;
using ChocolatesSV.Common;
using ChocolatesSV.DAL.Services;
using ChocolatesSV.BL.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuraciones existentes
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddControllers();

// 2. Conexión de DbContext para Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration["AppSettings:ConnectionString"]));

// 3. Servicios de Seguridad e Identity
builder.Services.AddAuthorization();
builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddIdentityCore<Usuario>()
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddApiEndpoints();

// 4. OpenAPI / Scalar
builder.Services.AddOpenApi();

// 5. Inyección de dependencias de capas
builder.Services.AddRepositoryConnector();
builder.Services.AddServiceConnector();

var app = builder.Build();

// 6. Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

// 7. Middlewares de seguridad
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 8. Endpoints de autenticación
app.MapGroup("/api/auth")
    .WithTags("Auth")
    .MapIdentityApi<Usuario>();

app.Run();