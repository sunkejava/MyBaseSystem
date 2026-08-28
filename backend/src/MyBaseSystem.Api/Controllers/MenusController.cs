using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

[ApiController, Authorize(Policy="menu.manage"), Route("api/v1/menu-management")]
public sealed class MenusController(IMenuApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<ApiResult<List<MenuDto>>> List(CancellationToken ct)=>ApiResult<List<MenuDto>>.Ok(await service.ListAsync(ct));
    [HttpPost] public async Task<ApiResult<Guid>> Create(SaveMenuRequest request,CancellationToken ct)=>ApiResult<Guid>.Ok(await service.CreateAsync(request,ct));
    [HttpPut("{id:guid}")] public async Task<ApiResult<object>> Update(Guid id,SaveMenuRequest request,CancellationToken ct){await service.UpdateAsync(id,request,ct);return ApiResult<object>.Ok(new{});}
    [HttpDelete("{id:guid}")] public async Task<ApiResult<object>> Delete(Guid id,CancellationToken ct){await service.DeleteAsync(id,ct);return ApiResult<object>.Ok(new{});}
}
