using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyBaseSystem.Application;

namespace MyBaseSystem.Api.Controllers;

[ApiController, Authorize, Route("api/v1/roles")]
public sealed class RolesController(IRoleApplicationService service) : ControllerBase
{
    [HttpGet] public async Task<ApiResult<IReadOnlyList<RoleDto>>> List(CancellationToken ct) => ApiResult<IReadOnlyList<RoleDto>>.Ok(await service.ListAsync(ct));
    [HttpPost] public async Task<ApiResult<Guid>> Create(SaveRoleRequest r,CancellationToken ct)=>ApiResult<Guid>.Ok(await service.CreateAsync(r,ct));
    [HttpPut("{id:guid}")] public async Task<ApiResult<object>> Update(Guid id,SaveRoleRequest r,CancellationToken ct){await service.UpdateAsync(id,r,ct);return ApiResult<object>.Ok(new{});}
    [HttpDelete("{id:guid}")] public async Task<ApiResult<object>> Delete(Guid id,CancellationToken ct){await service.DeleteAsync(id,ct);return ApiResult<object>.Ok(new{});}
}
