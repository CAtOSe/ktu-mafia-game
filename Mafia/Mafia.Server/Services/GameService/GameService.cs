using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
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
        //await AssignItems();

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

    /*private async Task AssignRoles()
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
    }*/
    private async Task AssignRoles()
    {
        string preset = "Basic";
        RoleFactorySelector roleFactorySelector = new RoleFactorySelector();
        RoleFactory roleFactory = roleFactorySelector.factoryMethod(preset);

        List<Role> killerRoles = roleFactory.GetKillerRoles();
        List<Role> accompliceRoles = roleFactory.GetAccompliceRoles();
        List<Role> citizenRoles = roleFactory.GetCitizenRoles();
        // Store the original citizen roles for duplication purposes
        List<Role> originalCitizenRoles = new List<Role>(citizenRoles);

        int accompliceCount = GetAccompliceCount(_currentPlayers.Count);

        var random = new Random();

        // Create a list to track unassigned players by index
        List<int> unassignedIndexes = Enumerable.Range(0, _currentPlayers.Count).ToList();

        // 1. Assign a random Killer role to one player
        int killerIndex = random.Next(unassignedIndexes.Count);
        var killerPlayer = _currentPlayers[unassignedIndexes[killerIndex]];

        // Get a random killer role from the killerRoles list
        var killerRole = killerRoles[random.Next(killerRoles.Count)];
        killerPlayer.Role = killerRole;

        // Remove the assigned killer role and the player index from the tracking list
        killerRoles.Remove(killerRole);
        unassignedIndexes.RemoveAt(killerIndex);

        // 2. Assign accomplice roles
        for (int i = 0; i < accompliceCount; i++)
        {
            if (unassignedIndexes.Count == 0) break;  // Safety check: stop if no players are left to assign

            if (accompliceRoles.Count > 0) // If there are accomplice roles available
            {
                var accompliceRole = accompliceRoles[random.Next(accompliceRoles.Count)];
                int accompliceIndex = random.Next(unassignedIndexes.Count);
                var accomplicePlayer = _currentPlayers[unassignedIndexes[accompliceIndex]];

                accomplicePlayer.Role = accompliceRole;
                accompliceRoles.Remove(accompliceRole);
                unassignedIndexes.RemoveAt(accompliceIndex);  // Remove the player index
            }
            else // If no more accomplice roles, assign Lackey
            {
                int accompliceIndex = random.Next(unassignedIndexes.Count);
                var accomplicePlayer = _currentPlayers[unassignedIndexes[accompliceIndex]];
                accomplicePlayer.Role = new Lackey();  // Assign Lackey
                unassignedIndexes.RemoveAt(accompliceIndex);  // Remove the player index
            }
        }

        // 3. Randomly assign 0 to 2 players the Bystander role
        int bystanderCount = random.Next(3);  // Random number between 0 and 2
        for (int i = 0; i < bystanderCount && unassignedIndexes.Count > 0; i++)
        {
            int bystanderIndex = random.Next(unassignedIndexes.Count);
            var bystanderPlayer = _currentPlayers[unassignedIndexes[bystanderIndex]];
            bystanderPlayer.Role = new Bystander();  // Assign Bystander
            unassignedIndexes.RemoveAt(bystanderIndex);  // Remove the player index
        }

        // 4. Assign remaining players random roles from citizenRoles
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            var player = _currentPlayers[playerIndex];
            if (citizenRoles.Count > 0) // If there are citizen roles available
            {
                var citizenRole = citizenRoles[random.Next(citizenRoles.Count)];
                player.Role = citizenRole;
                citizenRoles.Remove(citizenRole);  // Remove assigned role
            }
            else
            {
                // If citizenRoles run out, assign a random role from the original citizenRoles list (allow duplicates)
            var randomCitizenRole = originalCitizenRoles[random.Next(originalCitizenRoles.Count)];
            player.Role = randomCitizenRole;  // Allow duplicates now
            }
        }

        // Notify each player of their assigned role
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.RoleName}");
            await player.SendMessage(new Message
            {
                Base = ResponseCommands.RoleAssigned,
                Arguments = new string[] { player.RoleName }
            });
        }
    }


    private int GetAccompliceCount(int playerCount)
    {
        if (playerCount <= 5) return 0;
        if (playerCount >= 6 && playerCount <= 9) return 1;
        if (playerCount >= 10 && playerCount <= 12) return 2;
        if (playerCount >= 13) return 3;

        return 0;
    }

   /* private async Task AssignItems()
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
    }*/
}

