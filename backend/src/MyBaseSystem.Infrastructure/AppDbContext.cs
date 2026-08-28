using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // 业务聚合对应的 DbSet。新增实体时需同步配置索引、软删除过滤器和种子升级逻辑。
    public DbSet<User> Users => Set<User>(); public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>(); public DbSet<Menu> Menus => Set<Menu>();
    public DbSet<Department> Departments => Set<Department>(); public DbSet<DictionaryType> DictionaryTypes => Set<DictionaryType>();
    public DbSet<DictionaryItem> DictionaryItems => Set<DictionaryItem>(); public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>(); public DbSet<AuditLog> AuditLogs => Set<AuditLog>();
    public DbSet<LoginLog> LoginLogs => Set<LoginLog>();
    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        // SQLite cannot translate ordering/comparison for DateTimeOffset. Persist every
        // value as UTC ticks (INTEGER), keeping queries server-side and chronologically sortable.
        var dateTimeOffsetConverter = new ValueConverter<DateTimeOffset, long>(
            value => value.UtcTicks,
            value => new DateTimeOffset(value, TimeSpan.Zero));
        var nullableDateTimeOffsetConverter = new ValueConverter<DateTimeOffset?, long?>(
            value => value.HasValue ? value.Value.UtcTicks : null,
            value => value.HasValue ? new DateTimeOffset(value.Value, TimeSpan.Zero) : null);
        foreach (var entityType in b.Model.GetEntityTypes())
        foreach (var property in entityType.GetProperties())
        {
            if (property.ClrType == typeof(DateTimeOffset)) property.SetValueConverter(dateTimeOffsetConverter);
            else if (property.ClrType == typeof(DateTimeOffset?)) property.SetValueConverter(nullableDateTimeOffsetConverter);
        }
        foreach (var type in b.Model.GetEntityTypes().Where(x => typeof(Entity).IsAssignableFrom(x.ClrType)))
            b.Entity(type.ClrType).HasQueryFilter(BuildSoftDeleteFilter(type.ClrType));
        b.Entity<User>().HasIndex(x => x.UserName).IsUnique(); b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<Role>().HasIndex(x => x.Code).IsUnique(); b.Entity<Permission>().HasIndex(x => x.Code).IsUnique();
        b.Entity<Department>().HasIndex(x => x.Code).IsUnique().HasFilter("\"IsDeleted\" = 0"); b.Entity<DictionaryType>().HasIndex(x => x.Code).IsUnique();
        b.Entity<SystemSetting>().HasIndex(x => x.Key).IsUnique(); b.Entity<RefreshToken>().HasIndex(x => x.TokenHash).IsUnique();
        b.Entity<Menu>().HasIndex(x => x.Path).IsUnique().HasFilter("\"IsDeleted\" = 0");
        b.Entity<Notification>().HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
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
