namespace Mafia.Server.Models;

public record Message
{
    public string Base { get; init; }
    public string Error { get; init; }
    public IList<string> Arguments { get; init; }

    public static Message FromString(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return null;
        
        var mainParts = message.Split(':');
        if (mainParts.Length == 1)
        {
            return new Message
            {
                Base = mainParts[0],
                Arguments = [],
            };
        }

        var arguments = mainParts[1].Split(';');
        return new Message
        {
            Base = mainParts[0],
            Arguments = arguments
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray(),
        };
    }
}