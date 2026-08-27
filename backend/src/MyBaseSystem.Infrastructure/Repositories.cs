using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Application;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public sealed class EfUserRepository(AppDbContext db) : IUserRepository
{
    public async Task<(IReadOnlyList<User>,long)> SearchAsync(string? keyword,int page,int pageSize,CancellationToken ct){var q=db.Users.AsNoTracking().Include(x=>x.UserRoles).ThenInclude(x=>x.Role).AsQueryable();if(!string.IsNullOrWhiteSpace(keyword))q=q.Where(x=>x.UserName.Contains(keyword)||x.DisplayName.Contains(keyword)||x.Email.Contains(keyword));var total=await q.LongCountAsync(ct);var rows=await q.OrderByDescending(x=>x.CreatedAt).Skip((page-1)*pageSize).Take(pageSize).ToListAsync(ct);return(rows,total);}
    public Task<User?> GetWithRolesAsync(Guid id,CancellationToken ct)=>db.Users.Include(x=>x.UserRoles).FirstOrDefaultAsync(x=>x.Id==id,ct);
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
public sealed class AspNetPasswordService(PasswordHasher<User> hasher) : IPasswordService { public string Hash(User user,string password)=>hasher.HashPassword(user,password); }
