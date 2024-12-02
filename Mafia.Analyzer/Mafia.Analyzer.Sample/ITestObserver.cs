namespace Mafia.Analyzer.Sample;

public interface ITestObserver
{
    public void Log(string level, string message, string dateTime);
}