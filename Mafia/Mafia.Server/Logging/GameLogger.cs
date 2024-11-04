namespace Mafia.Server.Logging;

public class GameLogger
{
    private static GameLogger _instance;
    public static GameLogger Instance
    {
        get
        {
            if (_instance is null) _instance = new GameLogger();
            return _instance;
        }
    }

    private readonly List<ILoggerObserver> _observers = [];
    
    public void Attach(ILoggerObserver logger)
    {
        _observers.Add(logger);
    }

    public void Log(LogLevel level, string message)
    {
        var dateTime = DateTime.Now;
        _observers.ForEach(x => x.Log(level, message, dateTime));
    }

    public void Log(string message) => Log(LogLevel.Information, message);
}