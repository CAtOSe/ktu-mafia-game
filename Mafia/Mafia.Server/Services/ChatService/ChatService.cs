using System.Text.Json;
using Mafia.Server.Models;
using Mafia.Server.Models.Bridge;
using Mafia.Server.Models.Messages;
using CommandMessage = Mafia.Server.Models.Messages.CommandMessage;

namespace Mafia.Server.Services.ChatService;

public class ChatService : IChatService
{
    List<ChatMessage> messages = new List<ChatMessage>();
    private readonly IMessageHandler _messageHandler;

    private List<Player> players; 
    
    public ChatService(IMessageHandler messageHandler)
    {
        _messageHandler = messageHandler;
    }

    public void SetPlayers(List<Player> newPlayers)
    {
        players = newPlayers;
    }

    public void ResetChat()
    {
        messages = new List<ChatMessage>();
    }

    public Task SendChatMessage(string sender, string content, string recipient, string category)
    {
        //Console.WriteLine($"We added a message from: {sender} who has written to chat: {content}");
        messages.Add(new ChatMessage(sender, content, recipient, category, messages.Count));
        return SendOutFilteredChats();
        /*var chatMessage = new ChatMessage(sender, content, recipient, category);
        chatMessage.ProcessMessage(_messageHandler);
        return Task.CompletedTask;*/
    }

    public Task SendChatMessage(ChatMessage chatMessage)
    {
        chatMessage.TimeSent = messages.Count;
        messages.Add(chatMessage);
        return SendOutFilteredChats();
    }

    public async Task SendOutFilteredChats()
    {
        foreach (Player p in players)
        {
            // 1. Only dead players should see "deadPlayer" messages
            // 2. Player should only get messages that are sent to them or everyone
            List<ChatMessage> filteredMessages = messages
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
        //Console.WriteLine("Serialized chat messages: " + customChatMessagesJson);
        await player.SendMessage(new CommandMessage
        {
            Base = ResponseCommands.ReceiveChatList,
            Arguments = new List<string> { customChatMessagesJson }
        });
    } 
    
    // New Method: Returns all messages (for testing purposes)
    public List<ChatMessage> GetMessages()
    {
        return messages;
    }

    // New Method: Filters messages for a specific player based on their status and recipient
    public List<ChatMessage> FilterMessagesForPlayer(Player player)
    {
        return messages
            .Where(msg => (!player.IsAlive || msg.ChatCategory != "deadPlayer") &&
                          (msg.Recipient == player.Name || msg.Recipient == "everyone"))
            .ToList();
    }
    
    public void SendMessage(ChatMessage message)
    {
        _messageHandler.HandleChat(message);
    }
}