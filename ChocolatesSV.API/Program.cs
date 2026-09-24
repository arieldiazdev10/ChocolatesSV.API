using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ChocolatesSV.Entities.Models;
using ChocolatesSV.DAL;
using Scalar.AspNetCore;
using ChocolatesSV.Common;
using ChocolatesSV.DAL.Services;
using ChocolatesSV.BL.Services;

var builder = WebApplication.CreateBuilder(args);

// 0. Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


// 1. Configuraciones existentes
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.AddControllers();

// 2. Conexión de DbContext para Identity
builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(builder.Configuration["AppSettings:ConnectionString"]));

// 3. Servicios de Seguridad e Identity
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ApplicationScheme;
})
.AddCookie(IdentityConstants.ApplicationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // Permite el intercambio entre el puerto 5173 y 7076
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Requiere HTTPS en el backend
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
})
.AddBearerToken(IdentityConstants.BearerScheme);

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

// 7. Uso de CORS
app.UseCors("AllowReactApp");

// 8. Middlewares de seguridad
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// 9. Endpoints de autenticación
app.MapGroup("/api/auth")
    .WithTags("Auth")
    .MapIdentityApi<Usuario>();

app.Run();