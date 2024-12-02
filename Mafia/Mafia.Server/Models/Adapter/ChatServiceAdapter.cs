using Mafia.Server.Services.ChatService;

namespace Mafia.Server.Models.Adapter
{
    public class ChatServiceAdapter(IChatService chatService) : IChatServiceAdapter
    {
        public Task SendMessage(string sender, string message, string recipient, string messageType)
        {
            return chatService.SendChatMessage(sender, message, recipient, messageType);
        }

        public async Task SendMessage(ChatMessage chatMessage)
        {
            await chatService.SendChatMessage(chatMessage);
        }
    }
}
