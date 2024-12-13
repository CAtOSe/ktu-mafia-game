namespace Mafia.Server.Models.Interpreter;

public class ArgumentExpression : ITerminalExpression
{
    public CommandInterpretContext Interpret(CommandInterpretContext context)
    {
        if (context.Terminate) return context;
        
        if (string.IsNullOrWhiteSpace(context.Input))
        {
            return context;
        }

        if (context.Result.Arguments is null)
            context.Result.Arguments = new List<string>();
        
        context.Result.Arguments.Add(context.Input);
        return context;
    }
}