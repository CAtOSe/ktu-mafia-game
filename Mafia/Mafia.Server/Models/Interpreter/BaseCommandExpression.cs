namespace Mafia.Server.Models.Interpreter;

public class BaseCommandExpression : ITerminalExpression
{
    public CommandInterpretContext Interpret(CommandInterpretContext context)
    {
        if (context.Terminate) return context;

        if (string.IsNullOrWhiteSpace(context.Input))
        {
            context.Terminate = true;
            context.Result = null;
            return context;
        }
        
        context.Result.Base = context.Input;
        return context;
    }
}