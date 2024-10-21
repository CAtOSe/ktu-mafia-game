using FluentAssertions;
using Mafia.Server.Models.Messages;

namespace Mafia.Server.Tests.Unit;

public class CommandMessageTests
{
    [Fact]
    public void CommandMessage_ShouldConstructFromString()
    {
        // Arrange
        var input = "base";
        
        // Act
        var result = CommandMessage.FromString(input);
        
        // Assert
        result.Base.Should().Be(input);
        result.Arguments.Should().BeEmpty();
    }
    
    [Fact]
    public void CommandMessage_ShouldConstructFromString_WithArguments()
    {
        // Arrange
        var input = "base:arg1;arg2";
        
        // Act
        var result = CommandMessage.FromString(input);
        
        // Assert
        result.Base.Should().Be("base");
        result.Arguments.Should().HaveCount(2);
        result.Arguments.Should().HaveElementAt(0, "arg1");
        result.Arguments.Should().HaveElementAt(1, "arg2");
    }
    
    [Fact]
    public void CommandMessage_ShouldConstructFromString_WithArgumentsFilterEmpty()
    {
        // Arrange
        var input = "base:arg1;;;arg2";
        
        // Act
        var result = CommandMessage.FromString(input);
        
        // Assert
        result.Base.Should().Be("base");
        result.Arguments.Should().HaveCount(2);
        result.Arguments.Should().HaveElementAt(0, "arg1");
        result.Arguments.Should().HaveElementAt(1, "arg2");
    }
    
    [Fact]
    public void CommandMessage_ShouldNotConstructFromString_FromEmptyString()
    {
        // Arrange
        var input = string.Empty;
        
        // Act
        var result = CommandMessage.FromString(input);
        
        // Assert
        result.Should().BeNull();
    }
}