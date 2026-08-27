using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MyBaseSystem.Application;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Infrastructure;

public sealed class JwtOptions { public const string Section = "Jwt"; public string Issuer { get; set; } = "MyBaseSystem"; public string Audience { get; set; } = "MyBaseSystem.Web"; public string Key { get; set; } = "CHANGE-THIS-DEVELOPMENT-KEY-AT-LEAST-32-BYTES"; public int AccessTokenMinutes { get; set; } = 30; public int RefreshTokenDays { get; set; } = 7; }

public sealed class TokenService(AppDbContext db, IOptions<JwtOptions> options, PasswordHasher<User> hasher) : ITokenService
{
    private readonly JwtOptions _options = options.Value;
    public async Task<TokenResponse?> LoginAsync(LoginRequest request, string? ip, string? userAgent, CancellationToken ct)
    {
        var user = await db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RolePermissions).ThenInclude(x => x.Permission)
            .FirstOrDefaultAsync(x => (x.UserName == request.Account || x.Email == request.Account) && x.IsEnabled, ct);
        var valid = user is not null && hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) != PasswordVerificationResult.Failed;
        db.LoginLogs.Add(new LoginLog { Account = request.Account, Success = valid, Message = valid ? "登录成功" : "账号或密码错误", IpAddress = ip, UserAgent = userAgent });
        if (!valid) { await db.SaveChangesAsync(ct); return null; }
        user!.LastLoginAt = DateTimeOffset.UtcNow; var response = CreateTokens(user, request.RememberMe ? 30 : _options.RefreshTokenDays);
        await db.SaveChangesAsync(ct); return response;
    }

    public async Task<TokenResponse?> RefreshAsync(string token, CancellationToken ct)
    {
        var hash = Hash(token); var stored = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == hash && x.RevokedAt == null && x.ExpiresAt > DateTimeOffset.UtcNow, ct);
        if (stored is null) return null;
        var user = await db.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).ThenInclude(x => x.RolePermissions).ThenInclude(x => x.Permission).FirstOrDefaultAsync(x => x.Id == stored.UserId && x.IsEnabled, ct);
        if (user is null) return null;
        stored.RevokedAt = DateTimeOffset.UtcNow; var response = CreateTokens(user, _options.RefreshTokenDays); stored.ReplacedByTokenHash = Hash(response.RefreshToken);
        await db.SaveChangesAsync(ct); return response;
    }

    public async Task RevokeAsync(string token, CancellationToken ct) { var item = await db.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == Hash(token), ct); if (item is not null) { item.RevokedAt = DateTimeOffset.UtcNow; await db.SaveChangesAsync(ct); } }

    private TokenResponse CreateTokens(User user, int refreshDays)
    {
        var roles = user.UserRoles.Select(x => x.Role.Code).Distinct().ToArray();
        var perms = user.UserRoles.SelectMany(x => x.Role.RolePermissions).Select(x => x.Permission.Code).Distinct().ToArray();
        var expires = DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var claims = new List<Claim> { new(JwtRegisteredClaimNames.Sub, user.Id.ToString()), new(ClaimTypes.Name, user.UserName), new(JwtRegisteredClaimNames.Email, user.Email), new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) };
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x))); claims.AddRange(perms.Select(x => new Claim("permission", x)));
        var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key)), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(_options.Issuer, _options.Audience, claims, expires: expires.UtcDateTime, signingCredentials: creds);
        var refresh = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        db.RefreshTokens.Add(new RefreshToken { UserId = user.Id, TokenHash = Hash(refresh), ExpiresAt = DateTimeOffset.UtcNow.AddDays(refreshDays) });
        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(jwt), refresh, expires, new(user.Id, user.UserName, user.DisplayName, user.Email, user.Avatar, roles, perms));
    }
    private static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value)));
}
