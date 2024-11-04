using System.Net.WebSockets;
using Mafia.Server.Models;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Tests.Unit.TestSetup;
using Moq;

namespace Mafia.Server.Tests.Unit.ServicesTests;

public class MessageResolverFacadeTests
{
    private readonly IMessageResolverFacade _sut;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly Mock<IChatService> _chatServiceMock;
    private readonly Mock<WebSocket> _socketMock;

    private readonly Player _testPlayer;

    public MessageResolverFacadeTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _chatServiceMock = new Mock<IChatService>();
        _socketMock = new Mock<WebSocket>();

        _sut = new MessageResolverFacade(_gameServiceMock.Object, _chatServiceMock.Object);

        _testPlayer = new Player(_socketMock.Object);
    }

    [Fact]
    public void MessageResolver_ShouldResolve_Login()
    {
        // Arrange
        var message = "login:my_username";
        _gameServiceMock.Setup(x => x.TryAddPlayer(_socketMock.Object, "my_username"));
        
        // Act
        _sut.HandleMessage(_socketMock.Object, message);
        
        // Assert
        _gameServiceMock.VerifyAll();
    }
    
    [Fact]
    public void MessageResolver_ShouldResolve_Disconnect()
    {
        // Arrange
        var message = "disconnect";
        _gameServiceMock.Setup(x => x.DisconnectPlayer(_socketMock.Object));
        
        // Act
        _sut.HandleMessage(_socketMock.Object, message);
        
        // Assert
        _gameServiceMock.VerifyAll();
    }
}