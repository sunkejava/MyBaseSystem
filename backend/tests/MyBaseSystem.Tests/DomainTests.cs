using FluentAssertions;
using MyBaseSystem.Domain;

namespace MyBaseSystem.Tests;

public sealed class DomainTests
{
    [Fact] public void System_admin_cannot_be_deleted(){var user=new User{UserName="admin",Email="a@b.com",PasswordHash="x"};var action=()=>user.Delete();action.Should().Throw<InvalidOperationException>();}
    [Fact] public void System_role_cannot_be_deleted(){var role=new Role{Code="admin",Name="管理员",IsSystem=true};var action=()=>role.Delete();action.Should().Throw<InvalidOperationException>();}
}
