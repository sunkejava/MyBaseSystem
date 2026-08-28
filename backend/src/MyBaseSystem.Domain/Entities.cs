namespace MyBaseSystem.Domain;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
}

public sealed class User : Entity
{
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public string DisplayName { get; set; } = "";
    public string? Avatar { get; set; }
    public string? Phone { get; set; }
    public Guid? DepartmentId { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTimeOffset? LastLoginAt { get; set; }
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public void UpdateProfile(string displayName,string email,string? phone,Guid? departmentId,bool enabled){DisplayName=displayName;Email=email;Phone=phone;DepartmentId=departmentId;IsEnabled=enabled;UpdatedAt=DateTimeOffset.UtcNow;}
    public void Delete(){if(UserName=="admin")throw new InvalidOperationException("内置管理员不能删除");IsDeleted=true;UpdatedAt=DateTimeOffset.UtcNow;}
}

public sealed class Role : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public bool IsSystem { get; set; }
    public bool IsEnabled { get; set; } = true;
    public ICollection<UserRole> UserRoles { get; set; } = [];
    public ICollection<RolePermission> RolePermissions { get; set; } = [];
    public void Rename(string name,string? description,bool enabled){Name=name;Description=description;IsEnabled=enabled;UpdatedAt=DateTimeOffset.UtcNow;}
    public void Delete(){if(IsSystem)throw new InvalidOperationException("系统角色不能删除");IsDeleted=true;UpdatedAt=DateTimeOffset.UtcNow;}
}

public sealed class Permission : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string Group { get; set; } = "system";
}

public sealed class Menu : Entity
{
    public Guid? ParentId { get; set; }
    public required string Name { get; set; }
    public required string Path { get; set; }
    public string? Component { get; set; }
    public string? Icon { get; set; }
    public string? PermissionCode { get; set; }
    public int Sort { get; set; }
    public MenuType Type { get; set; } = MenuType.Page;
    public bool Hidden { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public enum MenuType { Directory, Page, ExternalLink }

public sealed class Department : Entity
{
    public Guid? ParentId { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public sealed class DictionaryType : Entity
{
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsSystem { get; set; }
}

public sealed class DictionaryItem : Entity
{
    public Guid DictionaryTypeId { get; set; }
    public required string Label { get; set; }
    public required string Value { get; set; }
    public int Sort { get; set; }
    public bool IsEnabled { get; set; } = true;
}

public sealed class SystemSetting : Entity
{
    public required string Key { get; set; }
    public required string Value { get; set; }
    public string Group { get; set; } = "default";
    public string? Description { get; set; }
    public bool IsPublic { get; set; }
}

public sealed class RefreshToken : Entity
{
    public Guid UserId { get; set; }
    public required string TokenHash { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public string? ReplacedByTokenHash { get; set; }
}

public sealed class AuditLog : Entity
{
    public Guid? UserId { get; set; }
    public string UserName { get; set; } = "anonymous";
    public required string Method { get; set; }
    public required string Path { get; set; }
    public int StatusCode { get; set; }
    public long ElapsedMilliseconds { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Error { get; set; }
}

public sealed class LoginLog : Entity
{
    public string Account { get; set; } = "";
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

/// <summary>用户站内通知，支持已读状态和业务跳转。</summary>
public sealed class Notification : Entity
{
    public Guid? UserId { get; set; }
    public required string Title { get; set; }
    public required string Message { get; set; }
    public string Type { get; set; } = "info";
    public bool IsRead { get; set; }
    public string? ActionUrl { get; set; }
}

public sealed class UserRole { public Guid UserId { get; set; } public User User { get; set; } = null!; public Guid RoleId { get; set; } public Role Role { get; set; } = null!; }
public sealed class RolePermission { public Guid RoleId { get; set; } public Role Role { get; set; } = null!; public Guid PermissionId { get; set; } public Permission Permission { get; set; } = null!; }
