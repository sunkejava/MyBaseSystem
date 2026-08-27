using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

[ApiController, Route("api/v1/auth")]
public sealed class AuthController(ITokenService tokens) : ControllerBase
{
    [HttpPost("login"), AllowAnonymous]
    public async Task<ActionResult<ApiResult<TokenResponse>>> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await tokens.LoginAsync(request, HttpContext.Connection.RemoteIpAddress?.ToString(), Request.Headers.UserAgent, ct);
        return result is null ? Unauthorized(ApiResult<TokenResponse>.Fail("AUTH_INVALID_CREDENTIALS", "账号或密码错误")) : Ok(ApiResult<TokenResponse>.Ok(result));
    }
    [HttpPost("refresh"), AllowAnonymous]
    public async Task<ActionResult<ApiResult<TokenResponse>>> Refresh(RefreshRequest request, CancellationToken ct)
    { var result = await tokens.RefreshAsync(request.RefreshToken, ct); return result is null ? Unauthorized(ApiResult<TokenResponse>.Fail("AUTH_REFRESH_INVALID", "刷新令牌无效或已过期")) : Ok(ApiResult<TokenResponse>.Ok(result)); }
    [HttpPost("logout"), Authorize]
    public async Task<ActionResult<ApiResult<object>>> Logout(RefreshRequest request, CancellationToken ct) { await tokens.RevokeAsync(request.RefreshToken, ct); return Ok(ApiResult<object>.Ok(new { })); }
}
