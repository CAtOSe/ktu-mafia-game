using FluentAssertions;
using Mafia.Server.Models.AbstractFactory;

namespace Mafia.Server.Tests.Unit;

public class RoleFactorySelectorTests
{
    private RoleFactorySelector _sut = new();
    
    [Fact]
    public void RoleFactorySelector_ShouldSelectBasicFactory()
    {
        // Act
        var result = _sut.FactoryMethod("basic");
        
        // Assert
        result.Should().BeOfType<BasicRoleFactory>();
    }
    
    [Fact]
    public void RoleFactorySelector_ShouldSelectAdvancedFactory()
    {
        // Act
        var result = _sut.FactoryMethod("advanced");
        
        // Assert
        result.Should().BeOfType<AdvancedRoleFactory>();
    }
}