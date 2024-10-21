namespace Mafia.Server.Models.Messages;

public class JsonMessage : IMessage
{
    public object Content { get; set; }
}