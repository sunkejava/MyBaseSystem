using System.Net;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api;

public sealed class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try { await next(context); }
        catch (Exception ex)
        {
            logger.LogError(ex,"Unhandled request error {TraceId}",context.TraceIdentifier);
            var(status,code,message)=ex switch { DomainRuleException d=>((int)HttpStatusCode.BadRequest,d.Code,d.Message), KeyNotFoundException=>((int)HttpStatusCode.NotFound,"NOT_FOUND","资源不存在"), InvalidOperationException i=>((int)HttpStatusCode.BadRequest,"DOMAIN_RULE",i.Message), _=>((int)HttpStatusCode.InternalServerError,"INTERNAL_ERROR","服务器内部错误") };
            context.Response.StatusCode=status; context.Response.ContentType="application/json";
            await context.Response.WriteAsJsonAsync(new { success=false,data=(object?)null,message,code,traceId=context.TraceIdentifier });
        }
    }
}
