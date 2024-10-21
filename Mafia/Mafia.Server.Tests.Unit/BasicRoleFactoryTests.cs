using FluentAssertions;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;

namespace Mafia.Server.Tests.Unit;

public class BasicRoleFactoryTests
{
    private BasicRoleFactory _sut = new();

    [Fact]
    public void BasicRoleFactory_ShouldConstructKillerRoles()
    {
        // Act
        var result = _sut.GetKillerRoles();

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllBeOfType<Assassin>();
    }
    
    [Fact]
    public void BasicRoleFactory_ShouldConstructAccompliceRolesRoles()
    {
        // Act
        var result = _sut.GetAccompliceRoles();

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllBeOfType<Poisoner>();
    }
    
    [Fact]
    public void BasicRoleFactory_ShouldConstructCitizenRoles()
    {
        // Act
        var result = _sut.GetCitizenRoles();

        // Assert
        result.Should().HaveCount(3);
        result.Should().ContainSingle(x => x is Doctor);
        result.Should().ContainSingle(x => x is Tracker);
        result.Should().ContainSingle(x => x is Soldier);
    }
}