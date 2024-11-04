namespace Mafia.Server.Logging;

public interface ILoggerObserver
{
    public void Log(LogLevel level, string message, DateTime dateTime);
}