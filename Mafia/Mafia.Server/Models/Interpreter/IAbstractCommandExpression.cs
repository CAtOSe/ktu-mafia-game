namespace Mafia.Server.Models.Interpreter;

public interface IAbstractCommandExpression
{
    CommandInterpretContext Interpret(CommandInterpretContext context);
}

public interface INonTerminalExpression : IAbstractCommandExpression;

public interface ITerminalExpression : IAbstractCommandExpression;
