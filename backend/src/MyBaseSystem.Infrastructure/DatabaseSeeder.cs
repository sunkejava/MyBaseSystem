using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public static class DatabaseSeeder
{
    public static async Task InitializeAsync(AppDbContext db, PasswordHasher<User> hasher, CancellationToken ct = default)
    {
        Directory.CreateDirectory("data"); await db.Database.EnsureCreatedAsync(ct); if (await db.Users.AnyAsync(ct)) return;
        var codes = new[] { "system:user:list", "system:user:create", "system:user:update", "system:user:delete", "system:role:list", "system:role:manage", "system:menu:manage", "system:department:manage", "system:dictionary:manage", "system:setting:manage", "system:log:list" };
        var permissions = codes.Select(x => new Permission { Code = x, Name = x, Group = x.Split(':')[1] }).ToList();
        var role = new Role { Code = "admin", Name = "超级管理员", Description = "系统内置超级管理员", IsSystem = true };
        role.RolePermissions = permissions.Select(x => new RolePermission { Role = role, Permission = x }).ToList();
        var user = new User { UserName = "admin", Email = "admin@example.com", DisplayName = "系统管理员", PasswordHash = "" };
        user.PasswordHash = hasher.HashPassword(user, "Admin123!"); user.UserRoles.Add(new UserRole { User = user, Role = role });
        db.AddRange(permissions); db.Add(role); db.Add(user);
        db.Menus.AddRange(
            new Menu { Name="仪表盘", Path="/dashboard", Component="Dashboard", Icon="LayoutDashboard", Sort=10 },
            new Menu { Name="系统管理", Path="/system", Icon="Settings", Sort=20, Type=MenuType.Directory },
            new Menu { Name="用户管理", Path="/users/list/index", Component="Users", Icon="Users", PermissionCode="system:user:list", Sort=21 },
            new Menu { Name="角色管理", Path="/users/roles/list", Component="Roles", Icon="Shield", PermissionCode="system:role:list", Sort=22 },
            new Menu { Name="权限管理", Path="/users/permissions/list", Component="Permissions", Icon="KeyRound", PermissionCode="system:menu:manage", Sort=23 });
        await db.SaveChangesAsync(ct);
    }
}
