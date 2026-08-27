using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

[ApiController, Authorize, Route("api/v1/users")]
public sealed class UsersController(IUserApplicationService service) : ControllerBase
{
    [HttpGet]
    public async Task<ApiResult<PagedResult<UserListItem>>> List([FromQuery] string? keyword, [FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
    {
        return ApiResult<PagedResult<UserListItem>>.Ok(await service.ListAsync(keyword,page,pageSize,ct));
    }
    [HttpPost]
    public async Task<ActionResult<ApiResult<Guid>>> Create(SaveUserRequest request, CancellationToken ct)
    {
        return ApiResult<Guid>.Ok(await service.CreateAsync(request,ct));
    }
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ApiResult<object>>> Update(Guid id, SaveUserRequest request, CancellationToken ct)
    {
        await service.UpdateAsync(id,request,ct); return ApiResult<object>.Ok(new { });
    }
    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ApiResult<object>>> Delete(Guid id, CancellationToken ct) { await service.DeleteAsync(id,ct); return ApiResult<object>.Ok(new{}); }
}
