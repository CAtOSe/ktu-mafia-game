using Mafia.Server.Models.Adapter;
using Mafia.Server.Services.ChatService;

namespace Mafia.Server.Models.Mediator
{
    public class ChatMediator : IChatMediator
    {
        private readonly IChatServiceAdapter _chatAdapter;

        public ChatMediator(IChatServiceAdapter chatAdapter)
        {
            _chatAdapter = chatAdapter;
        }

        public Task SendMessage(string sender, string message, string recipient, string messageType)
        {
            var chatMessage = new ChatMessage(sender, message, recipient, messageType);
            return _chatAdapter.SendMessage(chatMessage);
        }

        public async Task SendMessage(ChatMessage chatMessage)
        {
            await _chatAdapter.SendMessage(chatMessage);
        }
    }

}
