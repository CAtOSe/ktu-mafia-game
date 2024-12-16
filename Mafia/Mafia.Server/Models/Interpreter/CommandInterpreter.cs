namespace Mafia.Server.Models.Interpreter;

public static class CommandInterpreter
{
    public static IAbstractCommandExpression GetInterpreter()
    {
        return new CommandSeparatorExpression(new BaseCommandExpression(),
            new ArgumentSeparatorExpression(new ArgumentExpression()));
    }
}