using Mafia.Server.Models;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Tests.Unit.TestSetup;
using Moq;

namespace Mafia.Server.Tests.Unit.ServicesTests;

public class MessageResolverTests
{
    private readonly IMessageResolver _sut;
    private readonly Mock<IGameService> _gameServiceMock;
    private readonly Mock<IChatService> _chatServiceMock;

    private readonly Player _testPlayer;

    public MessageResolverTests()
    {
        _gameServiceMock = new Mock<IGameService>();
        _chatServiceMock = new Mock<IChatService>();

        _sut = new MessageResolver(_gameServiceMock.Object, _chatServiceMock.Object);

        _testPlayer = new Player(MockWebSocket.Get());
    }

    [Fact]
    public void MessageResolver_ShouldResolve_Login()
    {
        // Arrange
        var message = "login:my_username";
        _gameServiceMock.Setup(x => x.TryAddPlayer(_testPlayer, "my_username"));
        
        // Act
        _sut.HandleMessage(_testPlayer, message);
        
        // Assert
        _gameServiceMock.VerifyAll();
    }
    
    [Fact]
    public void MessageResolver_ShouldResolve_Disconnect()
    {
        // Arrange
        var message = "disconnect";
        _gameServiceMock.Setup(x => x.DisconnectPlayer(_testPlayer));
        
        // Act
        _sut.HandleMessage(_testPlayer, message);
        
        // Assert
        _gameServiceMock.VerifyAll();
    }
}