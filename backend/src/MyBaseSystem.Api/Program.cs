using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Api;
using MyBaseSystem.Domain;
using MyBaseSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
// API 项目只负责宿主和中间件编排，业务逻辑位于 Application，数据实现位于 Infrastructure。
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor(); builder.Services.AddScoped<CurrentUser>();
builder.Services.AddControllers(); builder.Services.AddProblemDetails(); builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddCors(o => o.AddPolicy("web", p => p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:5173"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
// .NET 10 使用框架内置 OpenAPI，减少第三方生成器的兼容风险。
builder.Services.AddOpenApi();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
app.UseMiddleware<ExceptionMiddleware>(); app.UseMiddleware<AuditMiddleware>(); app.UseCors("web"); app.UseAuthentication(); app.UseAuthorization();
if (app.Environment.IsDevelopment()) app.MapOpenApi();
app.MapControllers(); app.MapHealthChecks("/health");
using (var scope = app.Services.CreateScope()) await DatabaseSeeder.InitializeAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), scope.ServiceProvider.GetRequiredService<PasswordHasher<User>>());
app.Run();

public partial class Program;
