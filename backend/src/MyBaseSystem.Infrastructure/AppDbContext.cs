using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>(); public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>(); public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Department> Departments => Set<Department>(); public DbSet<DictionaryType> DictionaryTypes => Set<DictionaryType>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>(); public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>(); public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        foreach (var type in b.Model.GetEntityTypes().Where(x => typeof(Entity).IsAssignableFrom(x.ClrType)))
            b.Entity(type.ClrType).HasQueryFilter(BuildSoftDeleteFilter(type.ClrType));
        b.Entity<User>().HasIndex(x => x.UserName).IsUnique(); b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Role>().HasIndex(x => x.Code).IsUnique(); b.Entity<Permission>().HasIndex(x => x.Code).IsUnique();
        b.Entity<Department>().HasIndex(x => x.Code).IsUnique(); b.Entity<DictionaryType>().HasIndex(x => x.Code).IsUnique();
        b.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique(); b.Entity<RefreshToken>().HasIndex(x => x.TokenHash).IsUnique();
        b.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        b.Entity<RolePermission>().HasKey(x => new { x.RoleId, x.PermissionId });
        b.Entity<UserRole>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
        b.Entity<UserRole>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
        b.Entity<RolePermission>().HasOne(x => x.Role).WithMany(x => x.RolePermissions).HasForeignKey(x => x.RoleId);
        b.Entity<RolePermission>().HasOne(x => x.Permission).WithMany().HasForeignKey(x => x.PermissionId);
    }

    private static System.Linq.Expressions.LambdaExpression BuildSoftDeleteFilter(Type type)
    {
        var p = System.Linq.Expressions.Expression.Parameter(type, "e");
        var body = System.Linq.Expressions.Expression.Equal(System.Linq.Expressions.Expression.Property(p, nameof(Entity.IsDeleted)), System.Linq.Expressions.Expression.Constant(false));
        return System.Linq.Expressions.Expression.Lambda(body, p);
    }
}
