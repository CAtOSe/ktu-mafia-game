namespace Mafia.Server.Models.Mediator
{
    public interface IChatMediator
    {
        Task SendMessage(string sender, string message, string recipient, string messageType);
        Task SendMessage(ChatMessage message);
    }
}
