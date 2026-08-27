using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using MyBaseSystem.Application;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.Section)); var jwt = config.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new();
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(config.GetConnectionString("Default") ?? "Data Source=data/my-base-system.db"));
        services.AddScoped<PasswordHasher<User>>(); services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, EfUserRepository>(); services.AddScoped<IRoleRepository, EfRoleRepository>(); services.AddScoped<ISystemReadRepository, EfSystemReadRepository>(); services.AddScoped<IPasswordService, AspNetPasswordService>();
        services.AddScoped<IUserApplicationService, UserApplicationService>(); services.AddScoped<IRoleApplicationService, RoleApplicationService>(); services.AddScoped<ISystemApplicationService, SystemApplicationService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => { o.MapInboundClaims = false; o.TokenValidationParameters = new() { ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)), ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30), NameClaimType = ClaimTypes.Name, RoleClaimType = ClaimTypes.Role }; });
        services.AddAuthorizationBuilder().AddPolicy("admin", p => p.RequireRole("admin")); return services;
    }
}
