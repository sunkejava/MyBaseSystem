using MyBaseSystem.Domain;

namespace MyBaseSystem.Application;

// 应用层只负责编排业务规则，不直接引用 EF Core、HTTP 或具体数据库。
// 控制器通过下方 ApplicationService 调用用例，数据访问则由仓储接口反转给基础设施层实现。

public sealed class UserApplicationService(IUserRepository users, IPasswordService passwords) : IUserApplicationService
{
    public async Task<PagedResult<UserListItem>> ListAsync(string? keyword,int page,int pageSize,CancellationToken ct){page=Math.Max(1,page);pageSize=Math.Clamp(pageSize,1,100);var(items,total)=await users.SearchAsync(keyword,page,pageSize,ct);var rows=new List<UserListItem>();foreach(var x in items){var department=await users.GetDepartmentAsync(x.DepartmentId,ct);rows.Add(new(x.Id,x.UserName,x.DisplayName,x.Email,x.Phone,x.IsEnabled,x.DepartmentId,department?.Name,x.UserRoles.Select(r=>r.RoleId).ToArray(),x.UserRoles.Select(r=>r.Role.Name).ToArray(),x.CreatedAt,x.LastLoginAt));}return new(rows,total,page,pageSize);}
    public async Task<Guid> CreateAsync(SaveUserRequest r,CancellationToken ct){if(await users.ExistsAsync(r.UserName,r.Email,null,ct))throw new DomainRuleException("USER_DUPLICATE","用户名或邮箱已存在");if(string.IsNullOrWhiteSpace(r.Password)||r.Password.Length<8)throw new DomainRuleException("PASSWORD_REQUIRED","初始密码至少需要 8 位");var u=new User{UserName=r.UserName,DisplayName=r.DisplayName,Email=r.Email,Phone=r.Phone,DepartmentId=r.DepartmentId,IsEnabled=r.IsEnabled,PasswordHash=""};u.PasswordHash=passwords.Hash(u,r.Password);u.UserRoles=r.RoleIds.Distinct().Select(x=>new UserRole{User=u,RoleId=x}).ToList();users.Add(u);await users.SaveChangesAsync(ct);return u.Id;}
    public async Task UpdateAsync(Guid id,SaveUserRequest r,CancellationToken ct){var u=await users.GetWithRolesAsync(id,ct)??throw new KeyNotFoundException();if(await users.ExistsAsync(r.UserName,r.Email,id,ct))throw new DomainRuleException("USER_DUPLICATE","用户名或邮箱已存在");u.UpdateProfile(r.DisplayName,r.Email,r.Phone,r.DepartmentId,r.IsEnabled);if(!string.IsNullOrWhiteSpace(r.Password))u.PasswordHash=passwords.Hash(u,r.Password);users.RemoveRoles(u.UserRoles);u.UserRoles=r.RoleIds.Distinct().Select(x=>new UserRole{UserId=id,RoleId=x}).ToList();await users.SaveChangesAsync(ct);}
    public async Task DeleteAsync(Guid id,CancellationToken ct){var u=await users.GetWithRolesAsync(id,ct)??throw new KeyNotFoundException();u.Delete();await users.SaveChangesAsync(ct);}
    public async Task ChangePasswordAsync(Guid id,ChangePasswordRequest r,CancellationToken ct){var user=await users.GetWithRolesAsync(id,ct)??throw new KeyNotFoundException();if(!passwords.Verify(user,r.CurrentPassword))throw new DomainRuleException("PASSWORD_INVALID","当前密码不正确");if(r.NewPassword.Length<8)throw new DomainRuleException("PASSWORD_WEAK","新密码至少需要 8 位");user.PasswordHash=passwords.Hash(user,r.NewPassword);user.UpdatedAt=DateTimeOffset.UtcNow;await users.SaveChangesAsync(ct);}
}

/// <summary>处理仪表盘、通知与系统设置等跨模块应用用例。</summary>
public sealed class OperationsApplicationService(IOperationsRepository repository) : IOperationsApplicationService
{
    public Task<DashboardSummary> SummaryAsync(Guid userId,CancellationToken ct)=>repository.SummaryAsync(userId,ct);
    public async Task<IReadOnlyList<NotificationDto>> NotificationsAsync(Guid userId,CancellationToken ct)=>(await repository.NotificationsAsync(userId,ct)).Select(x=>new NotificationDto(x.Id,x.Title,x.Message,x.Type,x.IsRead,x.ActionUrl,x.CreatedAt)).ToList();
    public async Task<Guid> CreateNotificationAsync(CreateNotificationRequest r,CancellationToken ct){if(string.IsNullOrWhiteSpace(r.Title)||string.IsNullOrWhiteSpace(r.Message))throw new DomainRuleException("NOTIFICATION_REQUIRED","通知标题和内容不能为空");var item=new Notification{Title=r.Title.Trim(),Message=r.Message.Trim(),Type=r.Type,ActionUrl=r.ActionUrl,UserId=r.UserId};repository.Add(item);await repository.SaveChangesAsync(ct);return item.Id;}
    public async Task MarkReadAsync(Guid id,Guid userId,CancellationToken ct){var item=await repository.NotificationAsync(id,userId,ct)??throw new KeyNotFoundException();item.IsRead=true;item.UpdatedAt=DateTimeOffset.UtcNow;await repository.SaveChangesAsync(ct);}
    public async Task MarkAllReadAsync(Guid userId,CancellationToken ct){foreach(var item in await repository.NotificationsAsync(userId,ct)){var tracked=await repository.NotificationAsync(item.Id,userId,ct);if(tracked is not null)tracked.IsRead=true;}await repository.SaveChangesAsync(ct);}
    public async Task DeleteNotificationAsync(Guid id,Guid userId,CancellationToken ct){var item=await repository.NotificationAsync(id,userId,ct)??throw new KeyNotFoundException();item.IsDeleted=true;await repository.SaveChangesAsync(ct);}
    public async Task<IReadOnlyList<SettingDto>> SettingsAsync(CancellationToken ct)=>(await repository.SettingsAsync(ct)).Select(x=>new SettingDto(x.Key,x.Value,x.Group,x.Description,x.IsPublic)).ToList();
    public async Task SaveSettingAsync(string key,SaveSettingRequest r,CancellationToken ct){var item=await repository.SettingAsync(key,ct);if(item is null){item=new SystemSetting{Key=key,Value=r.Value,Group=r.Group,Description=r.Description,IsPublic=r.IsPublic};repository.Add(item);}else{item.Value=r.Value;item.Group=r.Group;item.Description=r.Description;item.IsPublic=r.IsPublic;item.UpdatedAt=DateTimeOffset.UtcNow;}await repository.SaveChangesAsync(ct);}
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

public sealed class MenuApplicationService(IMenuRepository menus) : IMenuApplicationService
{
    public async Task<List<MenuDto>> ListAsync(CancellationToken ct)=>BuildTree(await menus.ListAsync(ct));
    public async Task<Guid> CreateAsync(SaveMenuRequest r,CancellationToken ct)
    {
        var value=await ValidateAsync(null,r,ct); var menu=new Menu{Name=r.Name,Path=r.Path}; Apply(menu,r,value); menus.Add(menu); await menus.SaveChangesAsync(ct); return menu.Id;
    }
    public async Task UpdateAsync(Guid id,SaveMenuRequest r,CancellationToken ct)
    {
        var menu=await menus.GetAsync(id,ct)??throw new KeyNotFoundException(); var value=await ValidateAsync(id,r,ct); Apply(menu,r,value); menu.UpdatedAt=DateTimeOffset.UtcNow; await menus.SaveChangesAsync(ct);
    }
    public async Task DeleteAsync(Guid id,CancellationToken ct)
    {
        var menu=await menus.GetAsync(id,ct)??throw new KeyNotFoundException(); if(await menus.HasChildrenAsync(id,ct))throw new DomainRuleException("MENU_HAS_CHILDREN","请先删除或移动子菜单"); menu.IsDeleted=true; menu.UpdatedAt=DateTimeOffset.UtcNow; await menus.SaveChangesAsync(ct);
    }
    private async Task<MenuType> ValidateAsync(Guid? id,SaveMenuRequest r,CancellationToken ct)
    {
        if(string.IsNullOrWhiteSpace(r.Name)||string.IsNullOrWhiteSpace(r.Path))throw new DomainRuleException("MENU_REQUIRED","菜单名称和路由地址不能为空");
        if(!Enum.TryParse<MenuType>(r.Type,true,out var type))throw new DomainRuleException("MENU_TYPE_INVALID","菜单类型无效");
        if(await menus.PathExistsAsync(r.Path.Trim(),id,ct))throw new DomainRuleException("MENU_PATH_DUPLICATE","路由地址已存在");
        if(r.ParentId==id)throw new DomainRuleException("MENU_PARENT_INVALID","菜单不能以自身作为上级");
        if(r.ParentId.HasValue)
        {
            var all=await menus.ListAsync(ct); if(all.All(x=>x.Id!=r.ParentId))throw new DomainRuleException("MENU_PARENT_NOT_FOUND","上级菜单不存在");
            for(var parent=r.ParentId;parent.HasValue;parent=all.FirstOrDefault(x=>x.Id==parent)?.ParentId) if(parent==id)throw new DomainRuleException("MENU_CYCLE","不能将菜单移动到自己的下级");
        }
        return type;
    }
    private static void Apply(Menu menu,SaveMenuRequest r,MenuType type){menu.ParentId=r.ParentId;menu.Name=r.Name.Trim();menu.Path=r.Path.Trim();menu.Component=Clean(r.Component);menu.Icon=Clean(r.Icon);menu.PermissionCode=Clean(r.PermissionCode);menu.Sort=r.Sort;menu.Type=type;menu.Hidden=r.Hidden;menu.IsEnabled=r.IsEnabled;}
    private static string? Clean(string? value)=>string.IsNullOrWhiteSpace(value)?null:value.Trim();
    private static List<MenuDto> BuildTree(IReadOnlyList<Menu> all){List<MenuDto> Build(Guid? parent)=>all.Where(x=>x.ParentId==parent).OrderBy(x=>x.Sort).ThenBy(x=>x.Name).Select(x=>new MenuDto(x.Id,x.ParentId,x.Name,x.Path,x.Component,x.Icon,x.PermissionCode,x.Sort,x.Type.ToString(),x.Hidden,x.IsEnabled,Build(x.Id))).ToList();return Build(null);}
}

public sealed class DepartmentApplicationService(IDepartmentRepository departments) : IDepartmentApplicationService
{
    public async Task<List<DepartmentDto>> ListAsync(CancellationToken ct)
    {
        var all=await departments.ListAsync(ct);var counts=await departments.UserCountsAsync(ct);List<DepartmentDto> Build(Guid? parent)=>all.Where(x=>x.ParentId==parent).OrderBy(x=>x.Sort).ThenBy(x=>x.Name).Select(x=>new DepartmentDto(x.Id,x.ParentId,x.Code,x.Name,x.Sort,x.IsEnabled,counts.GetValueOrDefault(x.Id),Build(x.Id))).ToList();return Build(null);
    }
    public async Task<Guid> CreateAsync(SaveDepartmentRequest r,CancellationToken ct){await ValidateAsync(null,r,ct);var item=new Department{Code=r.Code,Name=r.Name};Apply(item,r);departments.Add(item);await departments.SaveChangesAsync(ct);return item.Id;}
    public async Task UpdateAsync(Guid id,SaveDepartmentRequest r,CancellationToken ct){var item=await departments.GetAsync(id,ct)??throw new KeyNotFoundException();await ValidateAsync(id,r,ct);Apply(item,r);item.UpdatedAt=DateTimeOffset.UtcNow;await departments.SaveChangesAsync(ct);}
    public async Task DeleteAsync(Guid id,CancellationToken ct){var item=await departments.GetAsync(id,ct)??throw new KeyNotFoundException();if(await departments.HasChildrenAsync(id,ct))throw new DomainRuleException("DEPARTMENT_HAS_CHILDREN","请先删除或移动下级机构");if(await departments.HasUsersAsync(id,ct))throw new DomainRuleException("DEPARTMENT_HAS_USERS","该机构下仍有关联用户，不能删除");item.IsDeleted=true;item.UpdatedAt=DateTimeOffset.UtcNow;await departments.SaveChangesAsync(ct);}
    private async Task ValidateAsync(Guid? id,SaveDepartmentRequest r,CancellationToken ct){if(string.IsNullOrWhiteSpace(r.Code)||string.IsNullOrWhiteSpace(r.Name))throw new DomainRuleException("DEPARTMENT_REQUIRED","机构编码和名称不能为空");if(await departments.CodeExistsAsync(r.Code.Trim(),id,ct))throw new DomainRuleException("DEPARTMENT_CODE_DUPLICATE","机构编码已存在");if(r.ParentId==id)throw new DomainRuleException("DEPARTMENT_PARENT_INVALID","机构不能以自身作为上级");if(r.ParentId.HasValue){var all=await departments.ListAsync(ct);if(all.All(x=>x.Id!=r.ParentId))throw new DomainRuleException("DEPARTMENT_PARENT_NOT_FOUND","上级机构不存在");for(var parent=r.ParentId;parent.HasValue;parent=all.FirstOrDefault(x=>x.Id==parent)?.ParentId)if(parent==id)throw new DomainRuleException("DEPARTMENT_CYCLE","不能将机构移动到自己的下级");}}
    private static void Apply(Department item,SaveDepartmentRequest r){item.ParentId=r.ParentId;item.Code=r.Code.Trim();item.Name=r.Name.Trim();item.Sort=r.Sort;item.IsEnabled=r.IsEnabled;}
}
