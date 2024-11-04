namespace Mafia.Server.Logging;

public class ConsoleLoggerObserver : ILoggerObserver
{
    public void Log(LogLevel level, string message, DateTime dateTime)
    {
        // Filter out debug level logs
        if (level == LogLevel.Debug) return;
        
        Console.WriteLine($"{dateTime:s} [{level.ToString()}]: {message}");
    }
}