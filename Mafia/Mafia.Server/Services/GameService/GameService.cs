using Mafia.Server.Models;
using Mafia.Server.Models.Commands;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly List<Player> _currentPlayers = [];
    
    private CancellationTokenSource _cancellationTokenSource; //  Token for canceling the phase cycle
    private bool GameStarted { get; set; } = false;

    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = true;
    private volatile bool _hasExecutedNightAction = false;

    public List<Player> GetPlayers() => _currentPlayers;

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

        await AssignRoles();
        await AssignItems();

        await gameStartTask;
        GameStarted = true;
        StartDayNightCycle();
    }

    public async Task NightAction(Player actionUser, string actionTarget, string actionType)
    {
        if (_isDayPhase || _hasExecutedNightAction) return;
        
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

        _hasExecutedNightAction = true;

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
            await UpdateDayNightPhase();
            
            while (GameStarted && !token.IsCancellationRequested) 
            {
                if (_isDayPhase)
                {
                    await Task.Delay(GameConfiguration.DayPhaseDuration, token);
                }
                else
                {
                    _hasExecutedNightAction = false;
                    await Task.Delay(GameConfiguration.NightPhaseDuration, token);
                    _phaseCounter = _phaseCounter + 1;
                }

                _isDayPhase = !_isDayPhase; // Toggle between day and night 
                Console.WriteLine("Changing phase");
                await UpdateDayNightPhase();
            }
        }, token); 
    }
    
    private async Task UpdateDayNightPhase()
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

    private async Task AssignRoles()
    {
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
    }

    private async Task AssignItems()
    {
        foreach (var player in _currentPlayers)
        {
            player.Inventory.Add(new Item { Name = "Radio" });
            if (player.Role == PlayerRole.Killer)
            {
                player.Inventory.Add(new Item { Name = "Knife" });
            }
            
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.AssignItem,
                Arguments = player.Inventory.Select(x => x.Name).ToList()
            });
        }
    }
}

