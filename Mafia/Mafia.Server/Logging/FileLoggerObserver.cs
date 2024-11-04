namespace Mafia.Server.Logging;

public class FileLoggerObserver : ILoggerObserver
{
    private readonly string _logFilePath;
    
    public FileLoggerObserver(string baseFileName = "log.txt")
    {
        _logFilePath = GenerateUniqueLogFilePath(baseFileName);
        using (File.Create(_logFilePath)) { }
    }
    
    private string GenerateUniqueLogFilePath(string baseFileName)
    {
        var directory = Directory.GetCurrentDirectory();
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(baseFileName);
        var extension = Path.GetExtension(baseFileName);

        var counter = 1;
        var filePath = Path.Combine(directory, baseFileName);
        while (File.Exists(filePath))
        {
            filePath = Path.Combine(directory, $"{fileNameWithoutExtension}_{counter}{extension}");
            counter++;
        }

        return filePath;
    }

    public void Log(LogLevel level, string message, DateTime dateTime)
    {
        File.AppendAllText(_logFilePath,$"{dateTime:s} [{level.ToString()}]: {message}\n");
    }
}