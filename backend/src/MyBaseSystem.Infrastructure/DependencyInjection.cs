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
    /// <summary>注册数据库、认证授权、仓储和应用服务，是后端基础设施的唯一组合入口。</summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<JwtOptions>(config.GetSection(JwtOptions.Section)); var jwt = config.GetSection(JwtOptions.Section).Get<JwtOptions>() ?? new();
        services.AddDbContext<AppDbContext>(o => o.UseSqlite(config.GetConnectionString("Default") ?? "Data Source=data/my-base-system.db"));
        services.AddScoped<PasswordHasher<User>>(); services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserRepository, EfUserRepository>(); services.AddScoped<IRoleRepository, EfRoleRepository>(); services.AddScoped<ISystemReadRepository, EfSystemReadRepository>(); services.AddScoped<IPasswordService, AspNetPasswordService>();
        services.AddScoped<IMenuRepository, EfMenuRepository>(); services.AddScoped<IDepartmentRepository, EfDepartmentRepository>();
        services.AddScoped<IOperationsRepository, EfOperationsRepository>();
        services.AddScoped<IUserApplicationService, UserApplicationService>(); services.AddScoped<IRoleApplicationService, RoleApplicationService>(); services.AddScoped<ISystemApplicationService, SystemApplicationService>();
        services.AddScoped<IMenuApplicationService, MenuApplicationService>(); services.AddScoped<IDepartmentApplicationService, DepartmentApplicationService>();
        services.AddScoped<IOperationsApplicationService, OperationsApplicationService>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => { o.MapInboundClaims = false; o.TokenValidationParameters = new() { ValidIssuer = jwt.Issuer, ValidAudience = jwt.Audience, IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)), ValidateIssuer = true, ValidateAudience = true, ValidateIssuerSigningKey = true, ValidateLifetime = true, ClockSkew = TimeSpan.FromSeconds(30), NameClaimType = ClaimTypes.Name, RoleClaimType = ClaimTypes.Role }; });
        services.AddAuthorizationBuilder().AddPolicy("admin", p => p.RequireRole("admin"))
            .AddPolicy("menu.manage",p=>p.RequireClaim("permission","system:menu:manage"))
            .AddPolicy("department.manage",p=>p.RequireClaim("permission","system:department:manage")); return services;
    }
}
