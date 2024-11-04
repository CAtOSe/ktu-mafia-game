using System.Text.Json;
using Mafia.Server.Models;
using Mafia.Server.Models.Messages;
using CommandMessage = Mafia.Server.Models.Messages.CommandMessage;

namespace Mafia.Server.Services.ChatService;

public class ChatService : IChatService
{
    private List<ChatMessage> _messages = [];
    private List<Player> _players; 
    
    public void SetPlayers(List<Player> newPlayers)
    {
        _players = newPlayers;
    }

    public void ResetChat()
    {
        _messages = [];
    }

    public Task SendChatMessage(string sender, string content, string recipient, string category)
    {
        var chatMessage = new ChatMessage(sender, content, recipient, category, _messages.Count);
        _messages.Add(chatMessage);

        return SendOutFilteredChats();
    }

    public Task SendChatMessage(ChatMessage chatMessage)
    {
        chatMessage.TimeSent = _messages.Count;
        _messages.Add(chatMessage);
        return SendOutFilteredChats();
    }

    public async Task SendOutFilteredChats()
    {
        foreach (Player p in _players)
        {
            // 1. Only dead players should see "deadPlayer" messages
            // 2. Player should only get messages that are sent to them or everyone
            List<ChatMessage> filteredMessages = _messages
                .Where(msg => (!p.IsAlive || msg.ChatCategory != "deadPlayer") &&
                              (msg.Recipient == p.Name || msg.Recipient == "everyone"))
                .ToList();

            await SendChatList(p, filteredMessages);

        }
    }
    public async Task SendChatList(Player player, List<ChatMessage> chatList)
    {
        string chatMessagesJson = JsonSerializer.Serialize(chatList);
        string customChatMessagesJson = chatMessagesJson.Replace(":", "|");
        await player.SendMessage(new CommandMessage
        {
            Base = ResponseCommands.ReceiveChatList,
            Arguments = new List<string> { customChatMessagesJson }
        });
    } 
    
    // Returns all messages (for testing purposes)
    public List<ChatMessage> GetMessages()
    {
        return _messages;
    }

    // Filters messages for a specific player based on their status and recipient
    public List<ChatMessage> FilterMessagesForPlayer(Player player)
    {
        return _messages
            .Where(msg => (!player.IsAlive || msg.ChatCategory != "deadPlayer") &&
                          (msg.Recipient == player.Name || msg.Recipient == "everyone"))
            .ToList();
    }
}