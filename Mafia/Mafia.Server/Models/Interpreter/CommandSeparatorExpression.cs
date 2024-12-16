namespace Mafia.Server.Models.Interpreter;

public class CommandSeparatorExpression(
    IAbstractCommandExpression firstExpression,
    IAbstractCommandExpression secondExpression,
    char splitChar = ':') : INonTerminalExpression
{
    public CommandInterpretContext Interpret(CommandInterpretContext context)
    {
        if (context.Terminate) return context;
        
        if (context.Result.Arguments is null) 
            context.Result.Arguments = new List<string>();
        
        var splitString = context.Input.Split(splitChar);

        if (splitString.Length == 0)
        {
            throw new ArgumentException("Invalid input string. Cannot parse command");
        }

        if (splitString.Length >= 1)
        {
            context.Update(firstExpression.Interpret(context with { Input = splitString[0] }));

        }

        if (splitString.Length >= 2)
        {
            context.Update(secondExpression.Interpret(context with { Input = splitString[1] }));
        }

        return context;
    }
}