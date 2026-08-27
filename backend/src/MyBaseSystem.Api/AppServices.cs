using System.Diagnostics;
using System.Security.Claims;
using MyBaseSystem.Application;
using MyBaseSystem.Domain;
using MyBaseSystem.Infrastructure;

namespace MyBaseSystem.Api;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    private ClaimsPrincipal? Principal => accessor.HttpContext?.User;
    public Guid? Id => Guid.TryParse(Principal?.FindFirstValue("sub"), out var id) ? id : null;
    public string Name => Principal?.Identity?.Name ?? "anonymous";
    public bool HasPermission(string code) => Principal?.Claims.Any(x => x.Type == "permission" && x.Value == code) == true || Principal?.IsInRole("admin") == true;
}

public sealed class AuditMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        var sw = Stopwatch.StartNew(); string? error = null;
        try { await next(context); } catch (Exception ex) { error = ex.Message; throw; }
        finally
        {
            sw.Stop();
            if (!context.Request.Path.StartsWithSegments("/health") && !context.Request.Path.StartsWithSegments("/swagger"))
            {
                Guid? userId = Guid.TryParse(context.User.FindFirstValue("sub"), out var id) ? id : null;
                db.AuditLogs.Add(new AuditLog { UserId = userId, UserName = context.User.Identity?.Name ?? "anonymous", Method = context.Request.Method, Path = context.Request.Path, StatusCode = context.Response.StatusCode, ElapsedMilliseconds = sw.ElapsedMilliseconds, IpAddress = context.Connection.RemoteIpAddress?.ToString(), UserAgent = context.Request.Headers.UserAgent.ToString(), Error = error });
                try { await db.SaveChangesAsync(context.RequestAborted); } catch { /* audit must not break the request */ }
            }
        }
    }
}
