using FluentAssertions;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;

namespace Mafia.Server.Tests.Unit;

public class AdvancedRoleFactoryTests
{
    private AdvancedRoleFactory _sut = new();

    [Fact]
    public void AdvancedRoleFactory_ShouldConstructKillerRoles()
    {
        // Act
        var result = _sut.GetKillerRoles();

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllBeOfType<Hemlock>();
    }
    
    [Fact]
    public void AdvancedRoleFactory_ShouldConstructAccompliceRolesRoles()
    {
        // Act
        var result = _sut.GetAccompliceRoles();

        // Assert
        result.Should().HaveCount(1);
        result.Should().AllBeOfType<Poisoner>();
    }
    
    [Fact]
    public void AdvancedRoleFactory_ShouldConstructCitizenRoles()
    {
        // Act
        var result = _sut.GetCitizenRoles();

        // Assert
        result.Should().HaveCount(4);
        result.Should().ContainSingle(x => x is Doctor);
        result.Should().ContainSingle(x => x is Tracker);
        result.Should().ContainSingle(x => x is Soldier);
        result.Should().ContainSingle(x => x is Lookout);

    }
}