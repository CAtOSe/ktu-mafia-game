namespace Mafia.Server.Logging;

public class EmptyObserver
{
    public void Log(LogLevel level, string message, DateTime dateTime)
    {
        Console.WriteLine(message);
    }
}