namespace MyBaseSystem.Application;

public sealed record ApiResult<T>(bool Success, T? Data, string Message = "ok", string? Code = null, string? TraceId = null)
{
    public static ApiResult<T> Ok(T data, string message = "ok") => new(true, data, message);
    public static ApiResult<T> Fail(string code, string message) => new(false, default, message, code);
}

public sealed record PagedResult<T>(IReadOnlyList<T> Items, long Total, int Page, int PageSize);
public sealed record LoginRequest(string Account, string Password, bool RememberMe = false);
public sealed record RefreshRequest(string RefreshToken);
public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt, UserProfile User);
public sealed record UserProfile(Guid Id, string UserName, string DisplayName, string Email, string? Avatar, string[] Roles, string[] Permissions);
public sealed record UserListItem(Guid Id, string UserName, string DisplayName, string Email, string? Phone, bool IsEnabled, string[] Roles, DateTimeOffset CreatedAt);
public sealed record SaveUserRequest(string UserName, string DisplayName, string Email, string? Phone, string? Password, bool IsEnabled, Guid? DepartmentId, Guid[] RoleIds);
public sealed record RoleDto(Guid Id, string Code, string Name, string? Description, bool IsSystem, bool IsEnabled, string[] Permissions);
public sealed record SaveRoleRequest(string Code, string Name, string? Description, bool IsEnabled, string[] Permissions);
public sealed record MenuDto(Guid Id, Guid? ParentId, string Name, string Path, string? Component, string? Icon, string? PermissionCode, int Sort, string Type, bool Hidden, bool IsEnabled, List<MenuDto> Children);
public sealed record SaveMenuRequest(Guid? ParentId, string Name, string Path, string? Component, string? Icon, string? PermissionCode, int Sort, string Type, bool Hidden, bool IsEnabled);
public sealed record DepartmentDto(Guid Id, Guid? ParentId, string Code, string Name, int Sort, bool IsEnabled, int UserCount, List<DepartmentDto> Children);
public sealed record SaveDepartmentRequest(Guid? ParentId, string Code, string Name, int Sort, bool IsEnabled);

public interface ICurrentUser { Guid? Id { get; } string Name { get; } bool HasPermission(string code); }
public interface ITokenService { Task<TokenResponse?> LoginAsync(LoginRequest request, string? ip, string? userAgent, CancellationToken ct); Task<TokenResponse?> RefreshAsync(string token, CancellationToken ct); Task RevokeAsync(string token, CancellationToken ct); }
