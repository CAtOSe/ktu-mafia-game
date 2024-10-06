using System.Net.WebSockets;
using System.Text;
using Mafia.Server.Models;
using Mafia.Server.Models.Commands;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];
    
    private CancellationTokenSource _cancellationTokenSource; //  Token for canceling the phase cycle
    private bool GameStarted { get; set; } = false;

    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = false;


    public async Task DisconnectPlayer(Player player)
    {
        _currentPlayers.Remove(player);
        await player.SendMessage(new Message
        {
            Base = RequestCommands.Disconnect
        });
        player.CloseConnection();

        if (_currentPlayers.Count == 0)
        {
            ResetGame();
        }
        else
        { 
            await SendPlayerList();
        }
    }
    
    public async Task TryAddPlayer(Player player, string username)
    {
        if (player.IsLoggedIn)
        {
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.AlreadyLoggedIn,
            });
            return;
        }
        
        var usernameTaken = _currentPlayers.Any(p =>
            p.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (usernameTaken)
        {
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.UsernameTaken
            });
            return;
        }

        player.IsLoggedIn = true;
        player.Name = username;
        player.IsHost = _currentPlayers.Count == 0;
        
        _currentPlayers.Add(player);
        await player.SendMessage(new Message
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = player.IsHost ? ["host"] : null
        });
        await SendPlayerList();
    }
    
    private Task SendPlayerList()
    {
        var message = new Message
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = _currentPlayers.Select(p => p.Name).ToList(),
        };
        return NotifyAllPlayers(message);
    }


    public async Task StartGame()
    {
        if (GameStarted)
        {
            Console.WriteLine("Game already started");
            return;
        }

        if (_currentPlayers.Count < 2)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

        Console.WriteLine("Assigning roles and starting the countdown.");
        await NotifyAllPlayers(new Message
        {
            Base = ResponseCommands.StartCountdown,
            Arguments = [GameConfiguration.BeginCountdown.ToString()]
        });
        var gameStartTask = Task.Delay(GameConfiguration.BeginCountdown).ContinueWith(async t =>
        {
            await NotifyAllPlayers(new Message
            {
                Base = ResponseCommands.GameStarted,
            });
        });

        // Randomly setting Killer role to 1 player
        var random = new Random();
        var killerIndex = random.Next(_currentPlayers.Count);
        var killer = _currentPlayers[killerIndex];
        killer.Role = PlayerRole.Killer;

        // Setting Citizen role for other players
        foreach (var player in _currentPlayers)
        {
            if (player != killer)
            {
                player.Role = PlayerRole.Citizen;
            }

            // Notify each player of their role
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.RoleAssigned,
                Arguments = [player.RoleName]
            });
        }

        await gameStartTask;
        GameStarted = true;
        StartDayNightCycle();
    }

    public List<Player> GetPlayers()
    {
        return _currentPlayers.ToList();
    }
    
    public Dictionary<string, string> GetPlayerRoles()
    {
        return _currentPlayers.ToDictionary(player => player.Name, player => player.RoleName);
    }
    
    //UPDATES:

    public async Task HandleMessageFromPlayer(Player player, string messageContent)
    {
        var message = new Message
        {
            Base = "chat",
            Arguments = new List<string> { $"{player.Name}: {messageContent}" }
        };
        await NotifyAllPlayers(message);
    }
    public async Task HandleIncomingMessages(WebSocket webSocket, Player player, CancellationToken stoppingToken)
    {
        var buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open && !stoppingToken.IsCancellationRequested)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), stoppingToken);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            // Handle received message
            if (!string.IsNullOrEmpty(receivedMessage))
            {
                await HandleMessageFromPlayer(player, receivedMessage);
            }
        }
    }
    
    public async Task NightAction(Player actionUser, string actionTarget, string actionType)
    {
        // Find the target player
        var targetPlayer = _currentPlayers.FirstOrDefault(p => p.Name.Equals(actionTarget, StringComparison.OrdinalIgnoreCase));

        // Validate that both the action user and the target player exist
        if (targetPlayer == null)
        {
            Console.WriteLine("Invalid action: Either the user or the target player does not exist.");
            return;
        }

        // Check if the action type is "kill"
        if (actionType.Equals("kill", StringComparison.OrdinalIgnoreCase))
        {
            // Perform the kill action
            targetPlayer.IsAlive = false;

            Console.WriteLine($"{targetPlayer.Name} has been killed by {actionUser.Name}.");
        }

        // Send updated list of alive players
        await SendAlivePlayerList();
    }

    private Task SendAlivePlayerList()
    {
        var alivePlayers = _currentPlayers.Where(p => p.IsAlive).Select(p => p.Name).ToList();

        Console.WriteLine("Player status:");
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name}: {player.IsAlive}");
        }

        var message = new Message
        {
            Base = ResponseCommands.AlivePlayerListUpdate,
            Arguments = alivePlayers,
        };
        return NotifyAllPlayers(message);
    }
    
    private Task NotifyAllPlayers(Message message)
    {
        var notifications = _currentPlayers.Select(p => p.SendMessage(message));
        return Task.WhenAll(notifications);
    }

    private void ResetGame()
    {
        GameStarted = false;
        _phaseCounter = 1;
        _cancellationTokenSource?.Cancel(); // Stop the day/night cycle
    }

    // Timer logic for day/night cycle 
    private void StartDayNightCycle()
    {
        _cancellationTokenSource = new CancellationTokenSource(); 
        var token = _cancellationTokenSource.Token; 

        Task.Run(async () =>
        {
            while (GameStarted && !token.IsCancellationRequested) 
            {
                if (_isDayPhase)
                {
                    await Task.Delay(GameConfiguration.DayPhaseDuration, token);
                    _phaseCounter = _phaseCounter + 1;
                }
                else
                {
                    await Task.Delay(GameConfiguration.NightPhaseDuration, token);
                }

                _isDayPhase = !_isDayPhase; // Toggle between day and night 
                Console.WriteLine("Changing phase");
                await ChangeDayPhase();
            }
        }, token); 
    }
    
    private async Task ChangeDayPhase()
    {
        await SendAlivePlayerList();
        var phaseName = _isDayPhase ? "day" : "night";
        var timeoutDuration = _isDayPhase ? GameConfiguration.DayPhaseDuration : GameConfiguration.NightPhaseDuration;
        Console.WriteLine($"\nNew phase: {phaseName} {_phaseCounter}");
        await NotifyAllPlayers(new Message
        {
            Base = ResponseCommands.PhaseChange,
            Arguments = [phaseName, timeoutDuration.ToString(), _phaseCounter.ToString()]
        });
    }
}

