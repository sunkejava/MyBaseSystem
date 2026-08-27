using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Api;
using MyBaseSystem.Domain;
using MyBaseSystem.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor(); builder.Services.AddScoped<CurrentUser>();
builder.Services.AddControllers(); builder.Services.AddProblemDetails(); builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
builder.Services.AddCors(o => o.AddPolicy("web", p => p.WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? ["http://localhost:5173"]).AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
builder.Services.AddEndpointsApiExplorer(); builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
app.UseMiddleware<ExceptionMiddleware>(); app.UseMiddleware<AuditMiddleware>(); app.UseCors("web"); app.UseAuthentication(); app.UseAuthorization();
app.UseSwagger(); app.UseSwaggerUI(); app.MapControllers(); app.MapHealthChecks("/health");
using (var scope = app.Services.CreateScope()) await DatabaseSeeder.InitializeAsync(scope.ServiceProvider.GetRequiredService<AppDbContext>(), scope.ServiceProvider.GetRequiredService<PasswordHasher<User>>());
app.Run();

public partial class Program;
