using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MyBaseSystem.Domain;
using MyBaseSystem.Infrastructure;
using Xunit;

namespace MyBaseSystem.Tests;

public sealed class SqliteDateTimeOffsetTests
{
    [Fact]
    public async Task DateTimeOffset_can_be_ordered_and_compared_by_sqlite()
    {
        await using var connection=new SqliteConnection("Data Source=:memory:");await connection.OpenAsync();
        var options=new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db=new AppDbContext(options);await db.Database.EnsureCreatedAsync();
        var older=new AuditLog{Method="GET",Path="/older",CreatedAt=DateTimeOffset.UtcNow.AddHours(-1)};
        var newer=new AuditLog{Method="GET",Path="/newer",CreatedAt=DateTimeOffset.UtcNow};
        db.AuditLogs.AddRange(older,newer);await db.SaveChangesAsync();
        var ordered=await db.AuditLogs.OrderByDescending(x=>x.CreatedAt).Select(x=>x.Path).ToListAsync();
        var recent=await db.AuditLogs.CountAsync(x=>x.CreatedAt>older.CreatedAt);
        ordered.Should().Equal("/newer","/older");recent.Should().Be(1);
    }
}
