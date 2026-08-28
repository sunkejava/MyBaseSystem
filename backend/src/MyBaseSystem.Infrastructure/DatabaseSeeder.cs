using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Domain;
using System.Security.Cryptography;

namespace MyBaseSystem.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task InitializeAsync(AppDbContext db, PasswordHasher<User> hasher, CancellationToken ct = default)
    {
        Directory.CreateDirectory("data"); await db.Database.EnsureCreatedAsync(ct);
        // Upgrade databases created before DateTimeOffset values were stored as UTC ticks.
        foreach(var (table,column) in new[]{("Users","CreatedAt"),("Users","UpdatedAt"),("Users","LastLoginAt"),("Roles","CreatedAt"),("Roles","UpdatedAt"),("Permissions","CreatedAt"),("Permissions","UpdatedAt"),("Menus","CreatedAt"),("Menus","UpdatedAt"),("Departments","CreatedAt"),("Departments","UpdatedAt"),("DictionaryTypes","CreatedAt"),("DictionaryTypes","UpdatedAt"),("DictionaryItems","CreatedAt"),("DictionaryItems","UpdatedAt"),("SystemSettings","CreatedAt"),("SystemSettings","UpdatedAt"),("RefreshTokens","CreatedAt"),("RefreshTokens","UpdatedAt"),("RefreshTokens","ExpiresAt"),("RefreshTokens","RevokedAt"),("AuditLogs","CreatedAt"),("AuditLogs","UpdatedAt"),("LoginLogs","CreatedAt"),("LoginLogs","UpdatedAt")})
            await db.Database.ExecuteSqlRawAsync($"UPDATE \"{table}\" SET \"{column}\" = CAST((julianday(\"{column}\") - julianday('0001-01-01')) * 864000000000 AS INTEGER) WHERE \"{column}\" IS NOT NULL AND CAST(\"{column}\" AS TEXT) LIKE '%-%:%'",ct);
        var codes = new[] { "system:user:list", "system:user:create", "system:user:update", "system:user:delete", "system:role:list", "system:role:manage", "system:menu:manage", "system:department:manage", "system:dictionary:manage", "system:setting:manage", "system:log:list" };
        var existingCodes=await db.Permissions.Select(x=>x.Code).ToListAsync(ct);var permissions=codes.Where(x=>!existingCodes.Contains(x)).Select(x => new Permission { Code = x, Name = x, Group = x.Split(':')[1] }).ToList();db.AddRange(permissions);
        if(!await db.Users.AnyAsync(ct)){
        var role = new Role { Code = "admin", Name = "超级管理员", Description = "系统内置超级管理员", IsSystem = true };
        role.RolePermissions = permissions.Select(x => new RolePermission { Role = role, Permission = x }).ToList();
        var user = new User { UserName = "admin", Email = "admin@example.com", DisplayName = "系统管理员", PasswordHash = "" };
        var initialPassword=Environment.GetEnvironmentVariable("MYBASESYSTEM_ADMIN_PASSWORD");
        if(string.IsNullOrWhiteSpace(initialPassword)){initialPassword=Convert.ToBase64String(RandomNumberGenerator.GetBytes(18));Console.WriteLine($"[MyBaseSystem] Initial admin password: {initialPassword}");}
        user.PasswordHash = hasher.HashPassword(user, initialPassword); user.UserRoles.Add(new UserRole { User = user, Role = role });
        db.Add(role); db.Add(user);}else{var adminRole=await db.Roles.Include(x=>x.RolePermissions).FirstOrDefaultAsync(x=>x.Code=="admin",ct);if(adminRole is not null){var assigned=adminRole.RolePermissions.Select(x=>x.PermissionId).ToHashSet();foreach(var permission in permissions.Where(x=>!assigned.Contains(x.Id)))adminRole.RolePermissions.Add(new RolePermission{Role=adminRole,Permission=permission});}}
        var menuPaths=await db.Menus.Select(x=>x.Path).ToListAsync(ct);
        if(!menuPaths.Contains("/system")){
        var systemMenu=new Menu { Name="系统管理", Path="/system", Icon="Settings", Sort=20, Type=MenuType.Directory };
        var additions=new List<Menu>{systemMenu};if(!menuPaths.Contains("/dashboard"))additions.Add(new Menu { Name="仪表盘", Path="/dashboard", Component="Dashboard", Icon="LayoutDashboard", Sort=10 });additions.AddRange([
            new Menu { ParentId=systemMenu.Id, Name="用户管理", Path="/users/list/index", Component="Users", Icon="Users", PermissionCode="system:user:list", Sort=21 },
            new Menu { ParentId=systemMenu.Id, Name="角色管理", Path="/users/roles/list", Component="Roles", Icon="Shield", PermissionCode="system:role:list", Sort=22 },
            new Menu { ParentId=systemMenu.Id, Name="菜单管理", Path="/system/menus", Component="Menus", Icon="Menu", PermissionCode="system:menu:manage", Sort=23 },
            new Menu { ParentId=systemMenu.Id, Name="组织机构", Path="/system/departments", Component="Departments", Icon="Building2", PermissionCode="system:department:manage", Sort=24 },
            new Menu { ParentId=systemMenu.Id, Name="权限管理", Path="/users/permissions/list", Component="Permissions", Icon="KeyRound", PermissionCode="system:menu:manage", Sort=25 }]);db.Menus.AddRange(additions);}
        else {var systemId=await db.Menus.Where(x=>x.Path=="/system").Select(x=>x.Id).SingleAsync(ct);if(!menuPaths.Contains("/system/menus"))db.Menus.Add(new Menu{ParentId=systemId,Name="菜单管理",Path="/system/menus",Component="Menus",Icon="Menu",PermissionCode="system:menu:manage",Sort=23});if(!menuPaths.Contains("/system/departments"))db.Menus.Add(new Menu{ParentId=systemId,Name="组织机构",Path="/system/departments",Component="Departments",Icon="Building2",PermissionCode="system:department:manage",Sort=24});}
        if(!await db.Departments.AnyAsync(ct))db.Departments.Add(new Department{Code="HQ",Name="总部",Sort=10});
        await db.SaveChangesAsync(ct);
    }
}
