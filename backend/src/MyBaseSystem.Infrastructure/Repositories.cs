using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Application;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

// EF Core 仓储统一放在基础设施层：查询默认使用 AsNoTracking，写入实体才保持跟踪。
// 这样既遵循整洁架构依赖方向，也避免后台列表查询产生不必要的跟踪开销。

public sealed class EfUserRepository(AppDbContext db) : IUserRepository
{
    public async Task<(IReadOnlyList<User>,long)> SearchAsync(string? keyword,int page,int pageSize,CancellationToken ct){var q=db.Users.AsNoTracking().Include(x=>x.UserRoles).ThenInclude(x=>x.Role).AsQueryable();if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.UserName.Contains(keyword)||x.DisplayName.Contains(keyword)||x.Email.Contains(keyword));var total=await q.LongCountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct);return(rows,total);}
    public Task<User?> GetWithRolesAsync(Guid id,CancellationToken ct)=>db.Users.Include(x=>x.UserRoles).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<Department?> GetDepartmentAsync(Guid? id,CancellationToken ct)=>id.HasValue?db.Departments.AsNoTracking().FirstOrDefaultAsync(x=>x.Id==id.Value,ct):Task.FromResult<Department?>(null);
    public Task<bool> ExistsAsync(string userName,string email,Guid? exceptId,CancellationToken ct)=>db.Users.AnyAsync(x=>(x.UserName==userName||x.Email==email)&&x.Id!=exceptId,ct);
    public void Add(User user)=>db.Users.Add(user); public void RemoveRoles(IEnumerable<UserRole> roles)=>db.RemoveRange(roles); public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
public sealed class EfRoleRepository(AppDbContext db) : IRoleRepository
{
    public async Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct)=>await db.Roles.AsNoTracking().Include(x=>x.RolePermissions).ThenInclude(x=>x.Permission).OrderBy(x=>x.Name).ToListAsync(ct);
    public Task<Role?> GetAsync(Guid id,CancellationToken ct)=>db.Roles.Include(x=>x.RolePermissions).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<bool> CodeExistsAsync(string code,Guid? exceptId,CancellationToken ct)=>db.Roles.AnyAsync(x=>x.Code==code&&x.Id!=exceptId,ct);
    public async Task<IReadOnlyList<Permission>> FindPermissionsAsync(IEnumerable<string> codes,CancellationToken ct){var values=codes.Distinct().ToArray();return await db.Permissions.Where(x=>values.Contains(x.Code)).ToListAsync(ct);}
    public void Add(Role role)=>db.Roles.Add(role); public void RemovePermissions(IEnumerable<RolePermission> items)=>db.RemoveRange(items); public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
public sealed class EfSystemReadRepository(AppDbContext db) : ISystemReadRepository
{
    public Task<User?> GetProfileAsync(Guid id,CancellationToken ct)=>db.Users.AsNoTracking().Include(x=>x.UserRoles).ThenInclude(x=>x.Role).ThenInclude(x=>x.RolePermissions).ThenInclude(x=>x.Permission).FirstOrDefaultAsync(x=>x.Id==id,ct);
    public async Task<IReadOnlyList<Permission>> ListPermissionsAsync(CancellationToken ct)=>await db.Permissions.AsNoTracking().OrderBy(x=>x.Group).ThenBy(x=>x.Code).ToListAsync(ct);
    public async Task<IReadOnlyList<Menu>> ListMenusAsync(CancellationToken ct)=>await db.Menus.AsNoTracking().OrderBy(x=>x.Sort).ToListAsync(ct);
    public async Task<(IReadOnlyList<AuditLog>,long)> ListAuditLogsAsync(int page,int pageSize,CancellationToken ct){var q=db.AuditLogs.AsNoTracking().OrderByDescending(x=>x.CreatedAt);return(await q.Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct),await q.LongCountAsync(ct));}
}
public sealed class AspNetPasswordService(PasswordHasher<User> hasher) : IPasswordService { public string Hash(User user,string password)=>hasher.HashPassword(user,password); public bool Verify(User user,string password)=>hasher.VerifyHashedPassword(user,user.PasswordHash,password)!=PasswordVerificationResult.Failed; }
public sealed class EfMenuRepository(AppDbContext db) : IMenuRepository
{
    public async Task<IReadOnlyList<Menu>> ListAsync(CancellationToken ct)=>await db.Menus.AsNoTracking().OrderBy(x=>x.Sort).ThenBy(x=>x.Name).ToListAsync(ct);
    public Task<Menu?> GetAsync(Guid id,CancellationToken ct)=>db.Menus.FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<bool> PathExistsAsync(string path,Guid? exceptId,CancellationToken ct)=>db.Menus.AnyAsync(x=>x.Path==path&&x.Id!=exceptId,ct);
    public Task<bool> HasChildrenAsync(Guid id,CancellationToken ct)=>db.Menus.AnyAsync(x=>x.ParentId==id,ct);
    public void Add(Menu menu)=>db.Menus.Add(menu); public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
public sealed class EfDepartmentRepository(AppDbContext db) : IDepartmentRepository
{
    public async Task<IReadOnlyList<Department>> ListAsync(CancellationToken ct)=>await db.Departments.AsNoTracking().OrderBy(x=>x.Sort).ThenBy(x=>x.Name).ToListAsync(ct);
    public Task<Department?> GetAsync(Guid id,CancellationToken ct)=>db.Departments.FirstOrDefaultAsync(x=>x.Id==id,ct);
    public Task<bool> CodeExistsAsync(string code,Guid? exceptId,CancellationToken ct)=>db.Departments.AnyAsync(x=>x.Code==code&&x.Id!=exceptId,ct);
    public Task<bool> HasChildrenAsync(Guid id,CancellationToken ct)=>db.Departments.AnyAsync(x=>x.ParentId==id,ct);
    public Task<bool> HasUsersAsync(Guid id,CancellationToken ct)=>db.Users.AnyAsync(x=>x.DepartmentId==id,ct);
    public async Task<IReadOnlyDictionary<Guid,int>> UserCountsAsync(CancellationToken ct)=>await db.Users.Where(x=>x.DepartmentId!=null).GroupBy(x=>x.DepartmentId!.Value).ToDictionaryAsync(x=>x.Key,x=>x.Count(),ct);
    public void Add(Department department)=>db.Departments.Add(department); public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}

/// <summary>仪表盘、通知和系统设置的数据访问实现。</summary>
public sealed class EfOperationsRepository(AppDbContext db) : IOperationsRepository
{
    public async Task<DashboardSummary> SummaryAsync(Guid userId,CancellationToken ct)
    {
        var today=new DateTimeOffset(DateTime.UtcNow.Date,TimeSpan.Zero);
        return new DashboardSummary(await db.Users.LongCountAsync(ct),await db.Users.LongCountAsync(x=>x.IsEnabled,ct),await db.Roles.LongCountAsync(ct),await db.Departments.LongCountAsync(ct),await db.LoginLogs.LongCountAsync(x=>x.Success&&x.CreatedAt>=today,ct),await db.AuditLogs.LongCountAsync(x=>x.CreatedAt>=today,ct),await db.Notifications.LongCountAsync(x=>(x.UserId==null||x.UserId==userId)&&!x.IsRead,ct));
    }
    public async Task<IReadOnlyList<Notification>> NotificationsAsync(Guid userId,CancellationToken ct)=>await db.Notifications.AsNoTracking().Where(x=>x.UserId==null||x.UserId==userId).OrderByDescending(x=>x.CreatedAt).Take(100).ToListAsync(ct);
    public Task<Notification?> NotificationAsync(Guid id,Guid userId,CancellationToken ct)=>db.Notifications.FirstOrDefaultAsync(x=>x.Id==id&&(x.UserId==null||x.UserId==userId),ct);
    public async Task<IReadOnlyList<SystemSetting>> SettingsAsync(CancellationToken ct)=>await db.SystemSettings.AsNoTracking().OrderBy(x=>x.Group).ThenBy(x=>x.Key).ToListAsync(ct);
    public Task<SystemSetting?> SettingAsync(string key,CancellationToken ct)=>db.SystemSettings.FirstOrDefaultAsync(x=>x.Key==key,ct);
    public void Add(Notification item)=>db.Notifications.Add(item); public void Add(SystemSetting item)=>db.SystemSettings.Add(item); public async Task SaveChangesAsync(CancellationToken ct)=>await db.SaveChangesAsync(ct);
}
