using Mafia.Server.Services.ChatService;

namespace Mafia.Server.Models.Adapter
{
    public class ChatServiceAdapter : IChatServiceAdapter
    {
        private IChatService _chatService;

        public ChatServiceAdapter(IChatService chatService)
        {
            _chatService = chatService;
        }

        public Task SendMessage(string sender, string message, string recipient, string messageType)
        {
            return _chatService.SendChatMessage(sender, message, recipient, messageType);
        }

        public async Task SendMessage(ChatMessage chatMessage)
        {
            await _chatService.SendChatMessage(chatMessage);
        }
    }
}
