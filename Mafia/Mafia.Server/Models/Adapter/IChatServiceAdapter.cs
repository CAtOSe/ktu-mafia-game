namespace Mafia.Server.Models.Adapter
{
    public interface IChatServiceAdapter
    {
        Task SendMessage(string sender, string message, string recipient, string messageType);
        Task SendMessage(ChatMessage chatMessage);
    }
}
