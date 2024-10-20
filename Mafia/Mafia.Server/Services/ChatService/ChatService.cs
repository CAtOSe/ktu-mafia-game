using System.Text.Json;
using Mafia.Server.Models;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.ChatService;

public class ChatService : IChatService
{
    List<ChatMessage> messages = new List<ChatMessage>();

    private List<Player> players; 

    public void SetPlayers(List<Player> newPlayers)
    {
        players = newPlayers;
    }
    public Task SendChatMessage(string sender, string content, string recipient, string category)
    {
        //Console.WriteLine($"We added a message from: {sender} who has written to chat: {content}");
        messages.Add(new ChatMessage(sender, content, recipient, category, messages.Count));
        return SendOutFilteredChats();
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
        await player.SendMessage(new Message
        {
            Base = ResponseCommands.ReceiveChatList,
            Arguments = new List<string> { customChatMessagesJson }
        });
    } 
}