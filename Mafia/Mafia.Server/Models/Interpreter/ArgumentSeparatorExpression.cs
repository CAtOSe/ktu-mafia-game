namespace Mafia.Server.Models.Interpreter;

public class ArgumentSeparatorExpression(IAbstractCommandExpression expression, char splitChar = ';')
    : INonTerminalExpression
{
    public CommandInterpretContext Interpret(CommandInterpretContext context)
    {
        if (context.Terminate) return context;
        
        var arguments = context.Input.Split(splitChar);

        foreach (var arg in arguments)
        {
            context.Update(expression.Interpret(context with { Input = arg }));
        }

        return context;
    }
}