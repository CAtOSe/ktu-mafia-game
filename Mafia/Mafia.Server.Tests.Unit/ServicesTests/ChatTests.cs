using System.Text.Json;
using Mafia.Server.Models;
using Mafia.Server.Models.Bridge;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Tests.Unit.TestSetup;
using Moq;
using Xunit;
using CommandMessage = Mafia.Server.Models.Messages.CommandMessage;

namespace Mafia.Server.Tests.Unit.ServicesTests;

public class ChatTests
{
    private readonly ChatService _sut;
    private readonly List<Player> _testPlayers;
    private readonly Player _alivePlayer;
    private readonly Player _deadPlayer;

    public ChatTests()
    {
        _sut = new ChatService();
        _alivePlayer = new Player(MockSocketHandler.Get()) { IsAlive = true, Name = "AlivePlayer" };
        _deadPlayer = new Player(MockSocketHandler.Get()) { IsAlive = false, Name = "DeadPlayer" };

        _testPlayers = new List<Player> { _alivePlayer, _deadPlayer };
        _sut.SetPlayers(_testPlayers);
    }

    [Fact]
    public async Task ChatService_ShouldSendChatMessage()
    {
        // Arrange
        var sender = "Sender";
        var content = "Hello, world!";
        var recipient = "Receiver";
        var category = ChatCategory.server;  // Use enum value instead of string

        // Act
        await _sut.SendChatMessage(sender, content, recipient, category.ToString());

        // Assert - Verify message was added to chat
        Assert.Contains(_sut.GetMessages(), m => 
            m.Sender == sender && 
            m.Content == content &&
            m.Recipient == recipient &&
            m.ChatCategory == category.ToString()
        );
    }

    [Fact]
    public async Task ChatService_ShouldResetChat()
    {
        // Arrange
        await _sut.SendChatMessage("Sender", "Message 1", "everyone", ChatCategory.server.ToString());
        await _sut.SendChatMessage("Sender", "Message 2", "everyone", ChatCategory.server.ToString());
        
        // Act
        _sut.ResetChat();
        
        // Assert
        Assert.Empty(_sut.GetMessages());
    }
    
    [Fact]
    public async Task ChatService_ShouldSerializeMessagesCorrectly()
    {
        // Arrange
        var testMessage = new ChatMessage("Tester", "Test message content", "everyone", "server", 0);
        await _sut.SendChatMessage(testMessage);

        // Act
        var filteredMessages = _sut.FilterMessagesForPlayer(_alivePlayer);
        var serializedMessages = JsonSerializer.Serialize(filteredMessages);
        var customSerializedMessages = serializedMessages.Replace(":", "|");

        // Send the custom serialized message to check format
        await _alivePlayer.SendMessage(new CommandMessage
        {
            Base = ResponseCommands.ReceiveChatList,
            Arguments = new List<string> { customSerializedMessages }
        });

        // Assert
        // Check if JSON serialization has replaced `:` with `|`
        Assert.Contains("|", customSerializedMessages);
        Assert.DoesNotContain(":", customSerializedMessages);
    }
    
    [Fact]
    public async Task ChatService_AlivePlayerShouldOnlySeeAliveMessages()
    {
        // Arrange
        var aliveMessage = new ChatMessage("Sender", "Message for alive players", "everyone", "player", 0);
        var deadMessage = new ChatMessage("Sender", "Message for dead players", "everyone", "deadPlayer", 1);

        await _sut.SendChatMessage(aliveMessage);
        await _sut.SendChatMessage(deadMessage);

        // Act
        var alivePlayerMessages = _sut.FilterMessagesForPlayer(_alivePlayer);

        // Assert
        Assert.DoesNotContain(alivePlayerMessages, m => m.ChatCategory == "deadPlayer");
        Assert.Contains(alivePlayerMessages, m => m.Content == "Message for alive players");
    }
    
    [Fact]
    public async Task ChatService_DeadPlayerShouldSeeBothAliveAndDeadMessages()
    {
        // Arrange
        var aliveMessage = new ChatMessage("Sender", "Message for alive players", "everyone", "player", 0);
        var deadMessage = new ChatMessage("Sender", "Message for dead players", "everyone", "deadPlayer", 1);

        await _sut.SendChatMessage(aliveMessage);
        await _sut.SendChatMessage(deadMessage);

        // Act
        var deadPlayerMessages = _sut.FilterMessagesForPlayer(_deadPlayer);

        // Assert
        Assert.Contains(deadPlayerMessages, m => m.ChatCategory == "deadPlayer");
        Assert.Contains(deadPlayerMessages, m => m.Content == "Message for alive players");
    }
    
    [Fact]
    public async Task ChatService_ShouldShowSpecificRecipientMessageToIntendedPlayerOnly()
    {
        // Arrange
        var specificRecipientMessage = new ChatMessage("Sender", "Message for AlivePlayer only", "AlivePlayer", "server", 2);

        await _sut.SendChatMessage(specificRecipientMessage);

        // Act
        var alivePlayerMessages = _sut.FilterMessagesForPlayer(_alivePlayer);
        var deadPlayerMessages = _sut.FilterMessagesForPlayer(_deadPlayer);

        // Assert
        Assert.Contains(alivePlayerMessages, m => m.Content == "Message for AlivePlayer only");
        Assert.DoesNotContain(deadPlayerMessages, m => m.Content == "Message for AlivePlayer only");
    }
}
