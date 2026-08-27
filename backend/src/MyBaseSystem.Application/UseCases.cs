using MyBaseSystem.Domain;

namespace MyBaseSystem.Application;

public sealed class UserApplicationService(IUserRepository users, IPasswordService passwords) : IUserApplicationService
{
    public async Task<PagedResult<UserListItem>> ListAsync(string? keyword,int page,int pageSize,CancellationToken ct){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var(items,total)=await users.SearchAsync(keyword,page,pageSize,ct);return new(items.Select(x=>new UserListItem(x.Id,x.UserName,x.DisplayName,x.Email,x.Phone,x.IsEnabled,x.UserRoles.Select(r=>r.Role.Name).ToArray(),x.CreatedAt)).ToList(),total,page,pageSize);}
    public async Task<Guid> CreateAsync(SaveUserRequest r,CancellationToken ct){if(await users.ExistsAsync(r.UserName,r.Email,null,ct))throw new DomainRuleException("USER_DUPLICATE","用户名或邮箱已存在");var u=new User{UserName=r.UserName,DisplayName=r.DisplayName,Email=r.Email,Phone=r.Phone,DepartmentId=r.DepartmentId,IsEnabled=r.IsEnabled,PasswordHash=""};u.PasswordHash=passwords.Hash(u,r.Password??"ChangeMe123!");u.UserRoles=r.RoleIds.Distinct().Select(x=>new UserRole{User=u,RoleId=x}).ToList();users.Add(u);await users.SaveChangesAsync(ct);return u.Id;}
    public async Task UpdateAsync(Guid id,SaveUserRequest r,CancellationToken ct){var u=await users.GetWithRolesAsync(id,ct)??throw new KeyNotFoundException();if(await users.ExistsAsync(r.UserName,r.Email,id,ct))throw new DomainRuleException("USER_DUPLICATE","用户名或邮箱已存在");u.UpdateProfile(r.DisplayName,r.Email,r.Phone,r.DepartmentId,r.IsEnabled);if(!string.IsNullOrWhiteSpace(r.Password))u.PasswordHash=passwords.Hash(u,r.Password);users.RemoveRoles(u.UserRoles);u.UserRoles=r.RoleIds.Distinct().Select(x=>new UserRole{UserId=id,RoleId=x}).ToList();await users.SaveChangesAsync(ct);}
    public async Task DeleteAsync(Guid id,CancellationToken ct){var u=await users.GetWithRolesAsync(id,ct)??throw new KeyNotFoundException();u.Delete();await users.SaveChangesAsync(ct);}
}

public sealed class RoleApplicationService(IRoleRepository roles) : IRoleApplicationService
{
    public async Task<IReadOnlyList<RoleDto>> ListAsync(CancellationToken ct)=>(await roles.ListAsync(ct)).Select(x=>new RoleDto(x.Id,x.Code,x.Name,x.Description,x.IsSystem,x.IsEnabled,x.RolePermissions.Select(p=>p.Permission.Code).ToArray())).ToList();
    public async Task<Guid> CreateAsync(SaveRoleRequest r,CancellationToken ct){if(await roles.CodeExistsAsync(r.Code,null,ct))throw new DomainRuleException("ROLE_DUPLICATE","角色编码已存在");var role=new Role{Code=r.Code,Name=r.Name,Description=r.Description,IsEnabled=r.IsEnabled};var ps=await roles.FindPermissionsAsync(r.Permissions,ct);role.RolePermissions=ps.Select(x=>new RolePermission{Role=role,Permission=x}).ToList();roles.Add(role);await roles.SaveChangesAsync(ct);return role.Id;}
    public async Task UpdateAsync(Guid id,SaveRoleRequest r,CancellationToken ct){var role=await roles.GetAsync(id,ct)??throw new KeyNotFoundException();role.Rename(r.Name,r.Description,r.IsEnabled);roles.RemovePermissions(role.RolePermissions);var ps=await roles.FindPermissionsAsync(r.Permissions,ct);role.RolePermissions=ps.Select(x=>new RolePermission{RoleId=id,PermissionId=x.Id}).ToList();await roles.SaveChangesAsync(ct);}
    public async Task DeleteAsync(Guid id,CancellationToken ct){var role=await roles.GetAsync(id,ct)??throw new KeyNotFoundException();role.Delete();await roles.SaveChangesAsync(ct);}
}

public sealed class SystemApplicationService(ISystemReadRepository data) : ISystemApplicationService
{
    public async Task<UserProfile> ProfileAsync(Guid id,CancellationToken ct){var u=await data.GetProfileAsync(id,ct)??throw new KeyNotFoundException();return new(u.Id,u.UserName,u.DisplayName,u.Email,u.Avatar,u.UserRoles.Select(x=>x.Role.Code).ToArray(),u.UserRoles.SelectMany(x=>x.Role.RolePermissions).Select(x=>x.Permission.Code).Distinct().ToArray());}
    public async Task<IReadOnlyList<object>> PermissionsAsync(CancellationToken ct)=>(await data.ListPermissionsAsync(ct)).Select(x=>(object)new{x.Id,x.Code,x.Name,x.Group,x.Description}).ToList();
    public async Task<List<MenuDto>> MenusAsync(IReadOnlySet<string> permissions,bool isAdmin,CancellationToken ct){var all=(await data.ListMenusAsync(ct)).Where(x=>x.IsEnabled&&(x.PermissionCode is null||isAdmin||permissions.Contains(x.PermissionCode))).ToList();List<MenuDto> Build(Guid? parent)=>all.Where(x=>x.ParentId==parent).Select(x=>new MenuDto(x.Id,x.ParentId,x.Name,x.Path,x.Component,x.Icon,x.PermissionCode,x.Sort,x.Type.ToString(),x.Hidden,x.IsEnabled,Build(x.Id))).ToList();return Build(null);}
    public async Task<PagedResult<object>> AuditLogsAsync(int page,int pageSize,CancellationToken ct){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var(items,total)=await data.ListAuditLogsAsync(page,pageSize,ct);return new(items.Select(x=>(object)new{x.Id,x.UserName,x.Method,x.Path,x.StatusCode,x.ElapsedMilliseconds,x.IpAddress,x.CreatedAt}).ToList(),total,page,pageSize);}
}
