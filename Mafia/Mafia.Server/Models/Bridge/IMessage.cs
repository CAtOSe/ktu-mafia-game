namespace Mafia.Server.Models.Bridge;

public interface IMessage
{
    string Sender { get; }
    string Content { get; }
    string Recipient { get; }
}
