using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Builder;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.ChatService;
using Microsoft.AspNetCore.Hosting.Server;
using System.Runtime.CompilerServices;

namespace Mafia.Server.Services.GameService;

public class GameService(IChatService _chatService) : IGameService
{

    private readonly List<Player> _currentPlayers = [];
    
    private CancellationTokenSource _cancellationTokenSource; //  Token for canceling the phase cycle
    private bool GameStarted { get; set; } = false;

    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = true;
    private volatile bool _hasExecutedNightAction = false;

    private List<NightAction> nightActions = new List<NightAction>();

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
        _chatService.SetPlayers(_currentPlayers);
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

    public async Task AddNightActionToList(Player actionUser, string actionTarget, string actionType)
    {
        // Find the target player
        var targetPlayer = _currentPlayers.FirstOrDefault(p => p.Name.Equals(actionTarget, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine("Received NIGHT ACTION: " + actionUser.Name + ", " + targetPlayer.Name);

        // Validate that both the action user and the target player exist
        if (targetPlayer == null)
        {
            Console.WriteLine("Invalid action: Either the user or the target player does not exist.");
            return;
        }
        // If it is the same action that we already have, treat it as canceling action
        NightAction previousAction = nightActions.FirstOrDefault(p => p.User == actionUser);
        bool cancelAction = previousAction != null && previousAction.Target == targetPlayer;

        // Remove any existing night actions from the same actionUser
        nightActions.RemoveAll(action => action.User == actionUser);

        // Add the new night action if it was not canceling the action
        if (!cancelAction)
        {
            nightActions.Add(new NightAction(actionUser, targetPlayer, actionType));
            await _chatService.SendChatMessage("","You have chosen " + targetPlayer.Name, actionUser.Name, "nightAction");
        }
        else
        {
            await _chatService.SendChatMessage("", "You have canceled your selection", actionUser.Name, "nightAction");
        }
    }

    public async Task VoteFor(Player player, string username)
    {
        var targetPlayer =
            _currentPlayers.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (!_isDayPhase || targetPlayer is null) return;

        if (player.CurrentVote != targetPlayer)
        { 
            await _chatService.SendChatMessage("","You have chosen " + targetPlayer.Name, player.Name, "dayAction"); 
            player.CurrentVote = targetPlayer;
        }
        else
        {
            await _chatService.SendChatMessage("", "You have canceled your selection", player.Name, "dayAction");
            player.CurrentVote = null;
        }
    }

    private async Task ExecuteNightActions()
    {
        // Define the custom order for roles
        var actionOrder = new List<string>
        {
            "Poisoner", "Tavern Keeper", "Tracker", "Lookout",
            "Assassin", "Hemlock", "Phantom", "Soldier", "Doctor"
        };

        // Sort nightActions by actionType based on the custom order
        nightActions = nightActions.OrderBy(action => actionOrder.IndexOf(action.Target.RoleName)).ToList();

        //List<(ChatMessage, int)> nightMessages = new List<(ChatMessage, int)>(); // 1 - Action // 2 - Death
        List<ChatMessage> nightMessages = new List<ChatMessage>();
        // Perform the night actions
        foreach (var nightAction in nightActions)
        {
            nightAction.User.Role.NightAction(nightAction.User, nightAction.Target, nightActions, nightMessages);
            nightAction.User.Role.AbilityUsesLeft--;
        }    

        // Send Messages about night actions
        foreach (var nightMessage in nightMessages)
        {
            await _chatService.SendChatMessage(nightMessage);
        }
        

        await SendAlivePlayerList();

        // Clear the nightActions list after processing all actions
        nightActions.Clear();
    }
    
    private async Task ExecuteDayActions()
    {
        var playerVotes = _currentPlayers.ToDictionary(x => x, _ => 0);
        
        foreach (var player in _currentPlayers.Where(player => player.CurrentVote is not null))
        {
            playerVotes[player.CurrentVote]++;
            player.CurrentVote = null;
        }

        var votes = playerVotes.OrderByDescending(x => x.Value).ToList();

        if (votes.Count == 0)
        {
            return;
        }
        
        if (votes.Count == 1 || votes[1].Value != votes[0].Value)
        {
            var votedOff = votes[0].Key;
            votedOff.IsAlive = false;
            await SendAlivePlayerList();
        }
    }

    
    private Task SendAlivePlayerList()
    {
        var alivePlayers = _currentPlayers.Where(p => p.IsAlive).Select(p => p.Name).ToList();

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
            // Send everyone chat message
            string chatMessageType = _isDayPhase ? "dayStart" : "nightStart";
            string phaseName = _isDayPhase ? "DAY" : "NIGHT";
            await _chatService.SendChatMessage("", phaseName + " " + _phaseCounter, "everyone", chatMessageType);

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


                if (_isDayPhase) //If it is the end of the night (start of day)
                {
                    Console.WriteLine("Executing night actions");
                    await ExecuteNightActions();
                }
                else
                {
                    await ExecuteDayActions();
                }
                // Send everyone chat message
                chatMessageType = _isDayPhase ? "dayStart" : "nightStart";
                phaseName = _isDayPhase ? "DAY" : "NIGHT";
                await _chatService.SendChatMessage("", phaseName + " " + _phaseCounter, "everyone", chatMessageType);

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
        string winnerTeam = DidAnyTeamWin();
        if (winnerTeam != null)
        {
            Console.WriteLine(winnerTeam + " team has won the game!");
            await _chatService.SendChatMessage("", winnerTeam + " team has won the game!", "everyone", "server");
            await NotifyAllPlayers(new Message
            {
                Base = ResponseCommands.EndGame,
                Arguments = [winnerTeam]
            });
            ResetGame();
        }
    }

    private string DidAnyTeamWin()
    {
        // Check if the Killer is dead for the Good team to win
        var killerPlayer = _currentPlayers.FirstOrDefault(player => player.RoleType == "Killer");
        if (killerPlayer != null && !killerPlayer.IsAlive)
        {
            return "Good";
        }

        // Count the number of alive Evil team members (Killer or Accomplice)
        int evilAlivePlayersCount = _currentPlayers.Count(player => player.IsAlive && (player.RoleType == "Killer" || player.RoleType == "Accomplice"));
        int totalAlivePlayersCount = _currentPlayers.Count(player => player.IsAlive);

        // Check if the evil team has the majority
        if (evilAlivePlayersCount > totalAlivePlayersCount / 2)
        {
            return "Evil";
        }

        // If no team has won yet
        return null;
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
        var killerRole = killerRoles[random.Next(killerRoles.Count)];

        // Builder
        IPlayerBuilder killerBuilder = new KillerBuilder(_currentPlayers[killerIndex].WebSocket);
        var killerPlayer = killerBuilder.SetName(_currentPlayers[killerIndex].Name)
                                        .SetRole(killerRole)
                                        .SetAlive(true)
                                        .SetLoggedIn(_currentPlayers[killerIndex].IsLoggedIn)
                                        .SetHost(_currentPlayers[killerIndex].IsHost)
                                        .Build();

        _currentPlayers[killerIndex] = killerPlayer;

        // Remove the assigned killer role and the player index from the tracking list
        killerRoles.Remove(killerPlayer.Role);
        unassignedIndexes.RemoveAt(killerIndex);

        // 2. Assign accomplice roles
        for (int i = 0; i < accompliceCount; i++)
        {
            if (unassignedIndexes.Count == 0) break;  // Safety check: stop if no players are left to assign

            int accompliceIndex = random.Next(unassignedIndexes.Count);
            Role accompliceRole = accompliceRoles.Count > 0
                                    ? accompliceRoles[random.Next(accompliceRoles.Count)]
                                    : new Lackey();
         
            // Builder
            IPlayerBuilder accompliceBuilder = new AccompliceBuilder(_currentPlayers[accompliceIndex].WebSocket);
            var accomplicePlayer = accompliceBuilder.SetName(_currentPlayers[accompliceIndex].Name)
                                                    .SetRole(accompliceRole)
                                                    .SetAlive(true)
                                                    .SetLoggedIn(_currentPlayers[accompliceIndex].IsLoggedIn)
                                                    .SetHost(_currentPlayers[accompliceIndex].IsHost)
                                                    .Build();

            _currentPlayers[accompliceIndex] = accomplicePlayer;
            accompliceRoles.Remove(accompliceRole);
            unassignedIndexes.RemoveAt(accompliceIndex);
        }

        // 3. Randomly assign 0 to 2 players the Bystander role
        int bystanderCount = random.Next(3);  // Random number between 0 and 2
        for (int i = 0; i < bystanderCount && unassignedIndexes.Count > 0; i++)
        {
            int bystanderIndex = random.Next(unassignedIndexes.Count);
            // Builder
            IPlayerBuilder citizenBuilder = new CitizenBuilder(_currentPlayers[bystanderIndex].WebSocket);
            var bystanderPlayer = citizenBuilder.SetName(_currentPlayers[bystanderIndex].Name)
                                                .SetRole(new Bystander())
                                                .SetAlive(true)
                                                .SetLoggedIn(_currentPlayers[bystanderIndex].IsLoggedIn)
                                                .SetHost(_currentPlayers[bystanderIndex].IsHost)
                                                .Build();

            _currentPlayers[bystanderIndex] = bystanderPlayer;
            unassignedIndexes.RemoveAt(bystanderIndex);
        }

        // 4. Assign remaining players random roles from citizenRoles
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            Role citizenRole = citizenRoles.Count > 0
                                ? citizenRoles[random.Next(citizenRoles.Count)]
                                : originalCitizenRoles[random.Next(originalCitizenRoles.Count)];
            // Builder
            IPlayerBuilder citizenBuilder = new CitizenBuilder(_currentPlayers[playerIndex].WebSocket);
            var citizenPlayer = citizenBuilder.SetName(_currentPlayers[playerIndex].Name)
                                              .SetRole(citizenRole)
                                              .SetAlive(true)
                                              .SetLoggedIn(_currentPlayers[playerIndex].IsLoggedIn)
                                              .SetHost(_currentPlayers[playerIndex].IsHost)
                                              .Build();

            _currentPlayers[playerIndex] = citizenPlayer;
            citizenRoles.Remove(citizenRole);
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

