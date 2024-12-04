using Mafia.Server.Controllers;
using Mafia.Server.Models;
using Mafia.Server.Models.Adapter;
using Mafia.Server.Models.Mediator;
using Mafia.Server.Models.Messages;
using Mafia.Server.Models.State;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Tests.Unit.TestSetup;
using Microsoft.Extensions.Time.Testing;
using Moq;

namespace Mafia.Server.Tests.Unit.ServicesTests;

public class GameServiceTests
{
    private readonly Mock<IChatService> _chatService;
    private readonly TimeProvider _timeProvider;
    private readonly Mock<IChatServiceAdapter> _chatAdapter;
    private readonly Mock<IChatMediator> _chatMediator;
    private readonly Mock<IGameStateManager> _stateManager;
    
    private readonly MockSocketHandler _mockSocketHandler;
    private readonly Player _testPlayer;
    
    private GameService _sut;
    
    public GameServiceTests()
    {
        _chatService = new Mock<IChatService>();
        _chatAdapter = new Mock<IChatServiceAdapter>();
        _chatMediator = new Mock<IChatMediator>();
        _timeProvider = new FakeTimeProvider();
        _stateManager = new Mock<IGameStateManager>();
        
        _mockSocketHandler = new MockSocketHandler();

        _sut = new GameService(
            _chatService.Object,
            _timeProvider,
            _chatAdapter.Object,
            new GameConfigurationFactoryMock(),
            _stateManager.Object,
            _chatMediator.Object);

        _testPlayer = new Player(_mockSocketHandler.Socket);
    }

    [Fact]
    public async Task GameService_TryAddPlayer_ShouldRespond()
    {
        // Arrange
        _mockSocketHandler.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = ["host"]
        });
        
        // Act
        await _sut.TryAddPlayer(_mockSocketHandler.Socket, TestData.TestUserName);
        
        // Assert
        _mockSocketHandler.Verify();
    }
    
    [Fact]
    public async Task GameService_TryAddPlayer_ShouldRespondOnlyOneHost()
    {
        // Arrange
        var player1Socket = new MockSocketHandler();
        var player2Socket = new MockSocketHandler();
        player1Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = ["host"]
        });
        player2Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = []
        });
        
        // Act
        await _sut.TryAddPlayer(player1Socket.Socket, TestData.TestUserName);
        await _sut.TryAddPlayer(player2Socket.Socket, TestData.TestUserName2);
        
        // Assert
        player1Socket.Verify();
        player2Socket.Verify();
    }
    
    [Fact]
    public async Task GameService_TryAddPlayer_ShouldReportNewPlayers()
    {
        // Arrange
        var player1Socket = new MockSocketHandler();
        var player2Socket = new MockSocketHandler();
        player1Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = [TestData.TestUserName]
        });
        player1Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = [TestData.TestUserName, TestData.TestUserName2]
        });
        
        player2Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = [TestData.TestUserName, TestData.TestUserName2]
        });
        
        // Act
        await _sut.TryAddPlayer(player1Socket.Socket, TestData.TestUserName);
        await _sut.TryAddPlayer(player2Socket.Socket, TestData.TestUserName2);
        
        // Assert
        player1Socket.Verify();
        player2Socket.Verify();
    }
    
    [Fact]
    public async Task GameService_TryAddPlayer_ShouldReportDuplicateUsername()
    {
        // Arrange
        var player2Socket = new MockSocketHandler();
        
        player2Socket.SetupExpectedMessage(new CommandMessage
        {
            Base = ResponseCommands.Error,
            Error = ErrorMessages.UsernameTaken
        });
        
        // Act
        await _sut.TryAddPlayer(_mockSocketHandler.Socket, TestData.TestUserName);
        await _sut.TryAddPlayer(player2Socket.Socket, TestData.TestUserName);
        
        // Assert
        player2Socket.Verify();
    }

    [Fact]
    public async Task GameService_StartGame_ShouldStart()
    {
        // Arrange
        var player1 = await SetupLoggedInUser(TestData.TestUserName);
        var player2 = await SetupLoggedInUser(TestData.TestUserName2);
        var player3 = await SetupLoggedInUser(TestData.TestUserName3);

        var countdownMessage = new CommandMessage
        {
            Base = ResponseCommands.StartCountdown,
            Arguments = [TestData.BeginCountdown.ToString()]
        };
        var gameStartMessage = new CommandMessage
        {
            Base = ResponseCommands.GameStarted,
        };
        
        player1.SetupExpectedMessage(countdownMessage);
        player2.SetupExpectedMessage(countdownMessage);
        player3.SetupExpectedMessage(countdownMessage);
        
        player1.SetupExpectedMessage(gameStartMessage);
        player2.SetupExpectedMessage(gameStartMessage);
        player3.SetupExpectedMessage(gameStartMessage);
        
        // Act
        await _sut.StartGame(TestData.TestDifficultyLevel);
        
        // Verify
        player1.Verify();
        player2.Verify();
        player3.Verify();
    }
    
    [Fact]
    public async Task GameService_StartGame_ShouldStart_OnlyWithEnoughPlayers()
    {
        // Arrange
        var player1 = await SetupLoggedInUser(TestData.TestUserName);
        var player2 = await SetupLoggedInUser(TestData.TestUserName2);
        
        // Act & Verify
        await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.StartGame(TestData.TestDifficultyLevel));
    }

    private async Task<MockSocketHandler> SetupLoggedInUser(string userName)
    {
        var socketHandler = new MockSocketHandler();

        await _sut.TryAddPlayer(socketHandler.Socket, userName);

        return socketHandler;
    }
}