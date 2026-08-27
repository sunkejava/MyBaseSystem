using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

[ApiController, Authorize, Route("api/v1")]
public sealed class SystemController(ISystemApplicationService service) : ControllerBase
{
    [HttpGet("auth/me")] public async Task<ApiResult<UserProfile>> Me(CancellationToken ct)=>ApiResult<UserProfile>.Ok(await service.ProfileAsync(Guid.Parse(User.FindFirst("sub")!.Value),ct));
    [HttpGet("permissions")] public async Task<ApiResult<IReadOnlyList<object>>> Permissions(CancellationToken ct)=>ApiResult<IReadOnlyList<object>>.Ok(await service.PermissionsAsync(ct));
    [HttpGet("menus")] public async Task<ApiResult<List<MenuDto>>> Menus(CancellationToken ct)=>ApiResult<List<MenuDto>>.Ok(await service.MenusAsync(User.Claims.Where(x=>x.Type=="permission").Select(x=>x.Value).ToHashSet(),User.IsInRole("admin"),ct));
    [HttpGet("audit-logs")] public async Task<ApiResult<PagedResult<object>>> Logs([FromQuery]int page=1,[FromQuery]int pageSize=20,CancellationToken ct=default)=>ApiResult<PagedResult<object>>.Ok(await service.AuditLogsAsync(page,pageSize,ct));
}
