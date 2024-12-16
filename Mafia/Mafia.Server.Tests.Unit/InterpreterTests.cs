using FluentAssertions;
using Mafia.Server.Models.Interpreter;
using Mafia.Server.Models.Messages;

namespace Mafia.Server.Tests.Unit;

public class InterpreterTests
{
    private readonly IAbstractCommandExpression _testInterpreter = CommandInterpreter.GetInterpreter();

    [Fact]
    public void CommandMessage_ShouldConstructFromString()
    {
        // Arrange
        var input = "base";
        var context = NewContext(input);
        
        // Act
        _testInterpreter.Interpret(context);
        
        // Assert
        context.Result.Base.Should().Be(input);
        context.Result.Arguments.Should().BeEmpty();
    }
    
    [Fact]
    public void CommandMessage_ShouldConstructFromString_WithArguments()
    {
        // Arrange
        var input = "base:arg1;arg2";
        var context = NewContext(input);
        
        // Act
        _testInterpreter.Interpret(context);
        
        // Assert
        context.Result.Base.Should().Be("base");
        context.Result.Arguments.Should().HaveCount(2);
        context.Result.Arguments.Should().HaveElementAt(0, "arg1");
        context.Result.Arguments.Should().HaveElementAt(1, "arg2");
    }
    
    [Fact]
    public void CommandMessage_ShouldConstructFromString_WithArgumentsFilterEmpty()
    {
        // Arrange
        var input = "base:arg1;;;arg2";
        var context = NewContext(input);
        
        // Act
        _testInterpreter.Interpret(context);
        
        // Assert
        context.Result.Base.Should().Be("base");
        context.Result.Arguments.Should().HaveCount(2);
        context.Result.Arguments.Should().HaveElementAt(0, "arg1");
        context.Result.Arguments.Should().HaveElementAt(1, "arg2");
    }
    
    [Fact]
    public void CommandMessage_ShouldNotConstructFromString_FromEmptyString()
    {
        // Arrange
        var input = string.Empty;
        var context = NewContext(input);
        
        // Act
        _testInterpreter.Interpret(context);
        
        // Assert
        context.Result.Should().BeNull();
    }

    private CommandInterpretContext NewContext(string input)
    {
        return new CommandInterpretContext
        {
            Input = input,
            Result = new CommandMessage()
        };
    }
}