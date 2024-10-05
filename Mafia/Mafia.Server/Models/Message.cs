using System.Text;
using Microsoft.Extensions.Primitives;

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

    public override string ToString()
    {
        var messageBuilder = new StringBuilder();
        messageBuilder.Append(Base);

        if (!string.IsNullOrWhiteSpace(Error))
        {
            messageBuilder.Append(':');
            messageBuilder.Append(Error);
        }
        else if (Arguments is not null && Arguments.Count > 0)
        {
            messageBuilder.Append(':');
            messageBuilder.Append(string.Join(';', Arguments));
        }

        return messageBuilder.ToString();
    }
}