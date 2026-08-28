using MyBaseSystem.Domain;

namespace MyBaseSystem.Application;

// Driven ports. Infrastructure owns their EF Core implementations.
public interface IUserRepository
{
    Task<(IReadOnlyList<User> Items, long Total)> SearchAsync(string? keyword, int page, int pageSize, CancellationToken ct);
    Task<User?> GetWithRolesAsync(Guid id, CancellationToken ct);
    Task<bool> ExistsAsync(string userName, string email, Guid? exceptId, CancellationToken ct);
    void Add(User user); void RemoveRoles(IEnumerable<UserRole> roles); Task SaveChangesAsync(CancellationToken ct);
}
public interface IRoleRepository
{
    Task<IReadOnlyList<Role>> ListAsync(CancellationToken ct); Task<Role?> GetAsync(Guid id, CancellationToken ct); Task<bool> CodeExistsAsync(string code, Guid? exceptId, CancellationToken ct);
    Task<IReadOnlyList<Permission>> FindPermissionsAsync(IEnumerable<string> codes, CancellationToken ct); void Add(Role role); void RemovePermissions(IEnumerable<RolePermission> items); Task SaveChangesAsync(CancellationToken ct);
}
public interface ISystemReadRepository
{
    Task<User?> GetProfileAsync(Guid id, CancellationToken ct); Task<IReadOnlyList<Permission>> ListPermissionsAsync(CancellationToken ct);
    Task<IReadOnlyList<Menu>> ListMenusAsync(CancellationToken ct); Task<(IReadOnlyList<AuditLog>, long)> ListAuditLogsAsync(int page, int pageSize, CancellationToken ct);
}
public interface IMenuRepository
{
    Task<IReadOnlyList<Menu>> ListAsync(CancellationToken ct); Task<Menu?> GetAsync(Guid id,CancellationToken ct);
    Task<bool> PathExistsAsync(string path,Guid? exceptId,CancellationToken ct); Task<bool> HasChildrenAsync(Guid id,CancellationToken ct);
    void Add(Menu menu); Task SaveChangesAsync(CancellationToken ct);
}
public interface IDepartmentRepository
{
    Task<IReadOnlyList<Department>> ListAsync(CancellationToken ct); Task<Department?> GetAsync(Guid id,CancellationToken ct);
    Task<bool> CodeExistsAsync(string code,Guid? exceptId,CancellationToken ct); Task<bool> HasChildrenAsync(Guid id,CancellationToken ct); Task<bool> HasUsersAsync(Guid id,CancellationToken ct);
    Task<IReadOnlyDictionary<Guid,int>> UserCountsAsync(CancellationToken ct); void Add(Department department); Task SaveChangesAsync(CancellationToken ct);
}
public interface IPasswordService { string Hash(User user, string password); }

// Driving ports. API only calls these use cases.
public interface IUserApplicationService { Task<PagedResult<UserListItem>> ListAsync(string? keyword, int page, int pageSize, CancellationToken ct); Task<Guid> CreateAsync(SaveUserRequest request, CancellationToken ct); Task UpdateAsync(Guid id, SaveUserRequest request, CancellationToken ct); Task DeleteAsync(Guid id, CancellationToken ct); }
public interface IRoleApplicationService { Task<IReadOnlyList<RoleDto>> ListAsync(CancellationToken ct); Task<Guid> CreateAsync(SaveRoleRequest request, CancellationToken ct); Task UpdateAsync(Guid id, SaveRoleRequest request, CancellationToken ct); Task DeleteAsync(Guid id, CancellationToken ct); }
public interface ISystemApplicationService { Task<UserProfile> ProfileAsync(Guid id, CancellationToken ct); Task<IReadOnlyList<object>> PermissionsAsync(CancellationToken ct); Task<List<MenuDto>> MenusAsync(IReadOnlySet<string> permissions, bool isAdmin, CancellationToken ct); Task<PagedResult<object>> AuditLogsAsync(int page, int pageSize, CancellationToken ct); }
public interface IMenuApplicationService { Task<List<MenuDto>> ListAsync(CancellationToken ct); Task<Guid> CreateAsync(SaveMenuRequest request,CancellationToken ct); Task UpdateAsync(Guid id,SaveMenuRequest request,CancellationToken ct); Task DeleteAsync(Guid id,CancellationToken ct); }
public interface IDepartmentApplicationService { Task<List<DepartmentDto>> ListAsync(CancellationToken ct); Task<Guid> CreateAsync(SaveDepartmentRequest request,CancellationToken ct); Task UpdateAsync(Guid id,SaveDepartmentRequest request,CancellationToken ct); Task DeleteAsync(Guid id,CancellationToken ct); }

public sealed class DomainRuleException(string code, string message) : Exception(message) { public string Code { get; } = code; }
