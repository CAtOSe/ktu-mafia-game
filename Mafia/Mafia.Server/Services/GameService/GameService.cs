using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using System.Text;
using Mafia.Server.Models.Strategy;

using System.Threading;
using System.Threading.Tasks;
using Mafia.Server.Models.Decorator;
using System.Xml.Linq;
using Mafia.Server.Models.Builder;
using System.Numerics;
using System.Linq;
using Mafia.Server.Models.Bridge;
using CommandMessage = Mafia.Server.Models.Messages.CommandMessage;
using Mafia.Server.Models.Adapter;
using Mafia.Server.Models.Facade;
using RoleFactorySelector = Mafia.Server.Models.AbstractFactory.RoleFactorySelector;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly IChatService _chatService;
    private readonly TimeProvider _timeProvider;
    private readonly IMessageHandler _messageHandler;
    private readonly IChatServiceAdapter _chatAdapter;
    private readonly GameRoleFacade _gameRoleFacade;

    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = true;

    private readonly List<Player> _currentPlayers = [];
    private List<ActionQueueEntry> _actionQueue = [];
    private List<ChatMessage> _dayStartAnnouncements = [];
    private List<Player> _playersWhoDiedInTheNight = [];
    private MorningAnnouncer _morningAnnouncer;

    private CancellationTokenSource _phaseCancelTokenSource; //  Token for canceling the phase cycle
    
    public List<Player> GetPlayers() => _currentPlayers;

    private DateTimeOffset _lastPhaseChange;
    private int _remainingPhaseTime;
    
    public bool IsPaused { get; private set; }
    public bool GameStarted { get; private set; }
    
    public GameService(IChatService chatService, TimeProvider timeProvider, 
                       IMessageHandler messageHandler, IChatServiceAdapter chatAdapter, GameRoleFacade gameRoleFacade)
    {
        IsPaused = false;
        _chatService = chatService;
        _timeProvider = timeProvider;
        _messageHandler = messageHandler;
        _chatAdapter = chatAdapter;
        _gameRoleFacade = gameRoleFacade;
    }
    
    public void AssignRole(string playerId, string presetName, string roleType)
    {
        // Use GameRoleFacade to assign role
        _gameRoleFacade.AssignRoleToPlayer(playerId, roleType);
    }
    public void ExecuteGameCommand(CommandMessage message)
    {
        _messageHandler.HandleCommand(message);
    }

    public void PauseTimer()
    {
        IsPaused = true;
        _phaseCancelTokenSource.Cancel();
        var phaseTime = _isDayPhase ? GameConfiguration.DayPhaseDuration : GameConfiguration.NightPhaseDuration;
        _remainingPhaseTime = phaseTime - (int) (_timeProvider.GetUtcNow() - _lastPhaseChange).TotalMilliseconds;
        var updateMessage = new CommandMessage
        {
            Base = ResponseCommands.GameUpdate,
            Arguments = new List<string> { "paused", _remainingPhaseTime.ToString() }
        };
        NotifyAllPlayers(updateMessage);
    }

    public void ResumeTimer()
    {
        IsPaused = false;
        var updateMessage = new CommandMessage
        {
            Base = ResponseCommands.GameUpdate,
            Arguments = new List<string> { "resumed", _remainingPhaseTime.ToString() }
        };
        NotifyAllPlayers(updateMessage);
        StartDayNightCycle(_remainingPhaseTime);
    }



    public async Task DisconnectPlayer(Player player)
    {
        _currentPlayers.Remove(player);
        await player.SendMessage(new CommandMessage
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
            if (player.IsHost)
            {
                _currentPlayers[0].IsHost = true;
                await _currentPlayers[0].SendMessage(new CommandMessage
                {
                    Base = ResponseCommands.LoggedIn,
                    Arguments = ["host"]
                });
            }
            
            await SendPlayerList();
        }
    }
    
    public async Task TryAddPlayer(Player player, string username)
    {
        if (player.IsLoggedIn)
        {
            await player.SendMessage(new CommandMessage
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
            await player.SendMessage(new CommandMessage
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
        await player.SendMessage(new CommandMessage
        {
            Base = ResponseCommands.LoggedIn,
            Arguments = player.IsHost ? ["host"] : null
        });
        await SendPlayerList();
    }
    
    private Task SendPlayerList()
    {
        _chatService.SetPlayers(_currentPlayers);
        var message = new CommandMessage
        {
            Base = ResponseCommands.PlayerListUpdate,
            Arguments = _currentPlayers.Select(p => p.Name).ToList(),
        };
        return NotifyAllPlayers(message);
    }


    public async Task StartGame(string difficultyLevel)
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
        await NotifyAllPlayers(new CommandMessage
        {
            Base = ResponseCommands.StartCountdown,
            Arguments = [GameConfiguration.BeginCountdown.ToString()]
        });
        var gameStartTask = Task.Delay(GameConfiguration.BeginCountdown).ContinueWith(async t =>
        {
            await NotifyAllPlayers(new CommandMessage
            {
                Base = ResponseCommands.GameStarted,
            });
        });

        await AssignRoles(difficultyLevel);

        await gameStartTask;
        GameStarted = true;
        StartDayNightCycle();
    }

    public async Task AddNightActionToList(Player actionUser, string actionTarget, string actionType)
    {
        actionUser = _currentPlayers.FirstOrDefault(p => p.Name.Equals(actionUser.Name, StringComparison.OrdinalIgnoreCase));
        // Find the target player
        var targetPlayer = _currentPlayers.FirstOrDefault(p => p.Name.Equals(actionTarget, StringComparison.OrdinalIgnoreCase));
        if (targetPlayer == null)
        {
            Console.WriteLine("Invalid action: Either the user or the target player does not exist.");
            return;
        }

        Console.WriteLine("Received NIGHT ACTION: " + actionUser.Name + " " + actionUser.Role + ", " + targetPlayer.Name);

        // If it is the same action that we already have, treat it as canceling action
        var previousAction = _actionQueue.FirstOrDefault(p => p.User == actionUser); 
        bool cancelAction = previousAction != null && previousAction.Target == targetPlayer;

        // Remove any existing night actions from the same actionUser
        _actionQueue.RemoveAll(action => action.User == actionUser);

        // Add the new night action if it was not canceling the action
        if (!cancelAction)
        {
            _actionQueue.Add(new ActionQueueEntry
            {
                User = actionUser,
                Target = targetPlayer
            });
            await _chatAdapter.SendMessage("","You have chosen " + targetPlayer.Name, actionUser.Name, "nightAction");
        }
        else
        {
            await _chatAdapter.SendMessage("", "You have canceled your selection", actionUser.Name, "nightAction");
        }
    }

    public async Task VoteFor(Player player, string username)
    {
        player = _currentPlayers.FirstOrDefault(x => x.Name.Equals(player.Name, StringComparison.OrdinalIgnoreCase));

        var targetPlayer =
            _currentPlayers.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (!_isDayPhase || targetPlayer is null) return;

        if (player.CurrentVote != targetPlayer)
        { 
            await _chatAdapter.SendMessage("","You have chosen " + targetPlayer.Name, player.Name, "dayAction"); 
            player.CurrentVote = targetPlayer;
        }
        else
        {
            await _chatAdapter.SendMessage("", "You have canceled your selection", player.Name, "dayAction");
            player.CurrentVote = null;
        }
    }

    private async Task ExecuteNightActions()
    {
        // Sort nightActions by actionType based on the custom order
        // nightActions = nightActions.OrderBy(action => actionOrder.IndexOf(action.Target.RoleName)).ToList();

        _actionQueue = _actionQueue.Select(a =>
        {
            var index = GameConfiguration.ActionOrder.IndexOf(a.User.Role.RoleAlgorithm.Name);
            var order = index == -1 ? int.MaxValue : index;
            return new KeyValuePair<int, ActionQueueEntry>(order, a);
        })
            .OrderBy(x => x.Key)
            .Select(x => x.Value)
            .ToList();

        // Dictionary to track players' alive status before the actions
        Dictionary<Player, bool> initialAliveStatus = new Dictionary<Player, bool>();

        foreach (var player in _currentPlayers)
        {
            player.IsPoisoned = false;
            initialAliveStatus[player] = player.IsAlive;
        }
        
        List<ChatMessage> nightMessages = new List<ChatMessage>();
        // Perform the night actions
        var context = new RoleActionContext
        {
            Players = _currentPlayers,
            QueuedActions = _actionQueue,
        };
        foreach (var nightAction in _actionQueue)
        {
            Console.WriteLine("Doing NIGHT ACTION: " + nightAction.User.RoleName + " " + nightAction.User.IsPoisoned);
            await nightAction.User.Role.ExecuteAction(nightAction.User, nightAction.Target, context, nightMessages);
            nightAction.User.Role.AbilityUsesLeft--;
        }

        // Send Messages about night actions
        foreach (var nightMessage in nightMessages)
        {
            await _chatAdapter.SendMessage(nightMessage);
        }

        int deathInNightCount = 0;
        foreach (var player in _currentPlayers)
        {
            // Check if the player died (alive status changed from true to false)
            if (initialAliveStatus[player] && !player.IsAlive)
            {
                deathInNightCount++;
                var deathMessage = new ChatMessage("", "You died.", player.Name, "nightNotification");
                _playersWhoDiedInTheNight.Add(player);
                //var dayAnnouncement = new ChatMessage("", player.Name + " has died in the night.", "everyone", "dayNotification");
                //_dayStartAnnouncements.Add(dayAnnouncement);
                await _chatAdapter.SendMessage(deathMessage);
            }
        }
        /*if(deathInNightCount == 0)
        {
            var dayAnnouncement = new ChatMessage("","No one has died in the night.", "everyone", "dayNotification");
            _dayStartAnnouncements.Add(dayAnnouncement);
        }*/

        await SendAlivePlayerList();

        // Clear the nightActions list after processing all actions
        _actionQueue.Clear();
    }

    private async Task ExecuteDayActions()
    {
        _morningAnnouncer.DayStartAnnouncements(_currentPlayers, _playersWhoDiedInTheNight, _dayStartAnnouncements); // DECORATOR
        //Sending "Player 1 has died in the night."
        foreach (ChatMessage announcement in _dayStartAnnouncements)
        {
            await _chatAdapter.SendMessage(announcement);
        }
        _dayStartAnnouncements.Clear();
        _playersWhoDiedInTheNight.Clear();
        
        var playerVotes = _currentPlayers.ToDictionary(x => x, _ => 0);
        
        foreach (var player in _currentPlayers.Where(player => player.CurrentVote is not null))
        {
            playerVotes[player.CurrentVote]++;
            player.CurrentVote = null;
        }

        var votes = playerVotes.OrderByDescending(x => x.Value).ToList();


        // Voting results message
        var votesResultBuilder = new StringBuilder();
        foreach (var vote in votes)
        {
            if (vote.Value != 0)
            {
                votesResultBuilder.AppendLine($"{vote.Key.Name} received {vote.Value} votes.");
            }
        }
        string votesResultMessage = votesResultBuilder.ToString();

        var finalVotesMessage = new ChatMessage("", votesResultMessage, "everyone", "server");
        await _chatAdapter.SendMessage(finalVotesMessage);

        if (votes.Count == 0) // No players voted
        {
            return;
        }
        
        if (votes.Count == 1 || votes[1].Value != votes[0].Value) // One max value
        {
            var votedOff = votes[0].Key;
            votedOff.IsAlive = false;
            await SendAlivePlayerList();
            List<ChatMessage> votingResultsMessages = new List<ChatMessage>();
            _morningAnnouncer.VotingEnd(votedOff, votingResultsMessages);
            //var votingResultMessage = new ChatMessage("", votedOff.Name + " has been voted off by the town.", "everyone", "dayNotification");
            foreach(var votingResultMessage in votingResultsMessages)
            {
                await _chatAdapter.SendMessage(votingResultMessage);
            }
            var votingResultPersonalMessage = new ChatMessage("", "You died.", votedOff.Name, "dayNotification");
            await _chatAdapter.SendMessage(votingResultPersonalMessage);
        }
        else // More than one max value
        {
            var votingResultMessage = new ChatMessage("", "No player has been voted of by the town today. (Tie)", "everyone", "dayNotification");
            await _chatAdapter.SendMessage(votingResultMessage);
        }
    }

    
    private Task SendAlivePlayerList()
    {
        var alivePlayers = _currentPlayers.Where(p => p.IsAlive).Select(p => p.Name).ToList();

        var message = new CommandMessage
        {
            Base = ResponseCommands.AlivePlayerListUpdate,
            Arguments = alivePlayers,
        };
        return NotifyAllPlayers(message);
    }
    
    internal Task NotifyAllPlayers(CommandMessage commandMessage)
    {
        var notifications = _currentPlayers.Select(p => p.SendMessage(commandMessage));
        return Task.WhenAll(notifications);
    }

    private void ResetGame()
    {
        IsPaused = false;
        GameStarted = false;
        _phaseCounter = 1;
        _chatService.ResetChat();
        _phaseCancelTokenSource?.Cancel(); // Stop the day/night cycle
    }

    // Timer logic for day/night cycle 
    private async void StartDayNightCycle(int remainingPhaseTime = 0)
    {
        _phaseCancelTokenSource = new CancellationTokenSource();
        var token = _phaseCancelTokenSource.Token;

        while (GameStarted && !token.IsCancellationRequested)
        {
            if (remainingPhaseTime == 0)
            {
                _lastPhaseChange = _timeProvider.GetUtcNow();
                string chatMessageType = _isDayPhase ? "dayStart" : "nightStart";
                string phaseName = _isDayPhase ? "DAY" : "NIGHT";
                await _chatAdapter.SendMessage("", phaseName + " " + _phaseCounter, "everyone",
                    chatMessageType); // DAY 1 / NIGHT 1
                await UpdateDayNightPhase();
            }
            else
            {
                var totalPhaseTime = _isDayPhase ?
                    GameConfiguration.DayPhaseDuration :
                    GameConfiguration.NightPhaseDuration;
                var elapsedPhaseTime = totalPhaseTime - remainingPhaseTime;
                _lastPhaseChange = _timeProvider.GetUtcNow().Subtract(TimeSpan.FromMilliseconds(elapsedPhaseTime));
            }
            
            try
            {
                if (_isDayPhase)
                {
                    await Task.Delay(remainingPhaseTime == 0 ? GameConfiguration.DayPhaseDuration : remainingPhaseTime,
                        token);
                    await ExecuteDayActions();
                }
                else
                {
                    await Task.Delay(
                        remainingPhaseTime == 0 ? GameConfiguration.NightPhaseDuration : remainingPhaseTime, token);
                    _phaseCounter = _phaseCounter + 1;
                    await ExecuteNightActions();
                }
            }
            catch (TaskCanceledException)
            {
                return;
            }

            _isDayPhase = !_isDayPhase;
            remainingPhaseTime = 0;
        }
    }

    
    private async Task UpdateDayNightPhase()
    {
        await SendAlivePlayerList();

        var phaseName = _isDayPhase ? "day" : "night";
        var timeoutDuration = _isDayPhase ? GameConfiguration.DayPhaseDuration : GameConfiguration.NightPhaseDuration;
        Console.WriteLine($"\nNew phase: {phaseName} {_phaseCounter}");
        await NotifyAllPlayers(new CommandMessage
        {
            Base = ResponseCommands.PhaseChange,
            Arguments = [phaseName, timeoutDuration.ToString(), _phaseCounter.ToString()]
        });
        string winnerTeam = DidAnyTeamWin();
        if (winnerTeam != null)
        {
            Console.WriteLine(winnerTeam + " team has won the game!");
            await _chatAdapter.SendMessage("", winnerTeam + " team has won the game!", "everyone", "server");
            await NotifyAllPlayers(new CommandMessage
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
    
    private async Task AssignRoles(string preset)
    {
        RoleFactorySelector roleFactorySelector = new RoleFactorySelector();
        RoleFactory roleFactory = roleFactorySelector.FactoryMethod(preset);

        // ABSTRACT FACTORY
        List<Role> killerRoles = roleFactory.GetKillerRoles();
        List<Role> accompliceRoles = roleFactory.GetAccompliceRoles();
        List<Role> citizenRoles = roleFactory.GetCitizenRoles();
        // DECORATOR
        _morningAnnouncer = roleFactory.GetAnnouncer();


        // PROTOTYPE
        List<Role> originalCitizenRoles = new List<Role>(citizenRoles);

        int accompliceCount = GetAccompliceCount(_currentPlayers.Count);

        var random = new Random();

        // Create a list to track unassigned players by index
        List<int> unassignedIndexes = Enumerable.Range(0, _currentPlayers.Count).ToList();

        // 1. Assign a random Killer role to one player
        int killerIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];

        // PROTOTYPE
        Role killerRole = (Role)killerRoles[random.Next(killerRoles.Count)].Clone();

        // BUILDER
        IPlayerBuilder killerBuilder = roleFactory.GetKillerBuilder(_currentPlayers[killerIndex].WebSocket);
        var killerPlayer = killerBuilder.SetName(_currentPlayers[killerIndex].Name)
                                        .SetRole(killerRole)
                                        .SetAlive(true)
                                        .SetLoggedIn(_currentPlayers[killerIndex].IsLoggedIn)
                                        .SetHost(_currentPlayers[killerIndex].IsHost)
                                        .Build();

        _currentPlayers[killerIndex] = killerPlayer;

        // Remove the assigned killer role and the player index from the tracking list
        killerRoles.Remove(killerPlayer.Role);
        unassignedIndexes.Remove(killerIndex);

        Console.WriteLine("Built Killer");
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.Role}");
        }

        // 2. Assign accomplice roles
        for (int i = 0; i < accompliceCount; i++)
        {
            if (unassignedIndexes.Count == 0) break;  // Safety check: stop if no players are left to assign

            int accompliceIndex = random.Next(unassignedIndexes.Count);

            // PROTOTYPE
            Role accompliceRole = accompliceRoles.Count > 0
                                    ? (Role)accompliceRoles[random.Next(accompliceRoles.Count)].Clone()
                                    : (Role)new Lackey().Clone();
            // BUILDER
            IPlayerBuilder accompliceBuilder = roleFactory.GetAccompliceBuilder(_currentPlayers[accompliceIndex].WebSocket);
            var accomplicePlayer = accompliceBuilder.SetName(_currentPlayers[accompliceIndex].Name)
                                                    .SetRole(accompliceRole)
                                                    .SetAlive(true)
                                                    .SetLoggedIn(_currentPlayers[accompliceIndex].IsLoggedIn)
                                                    .SetHost(_currentPlayers[accompliceIndex].IsHost)
                                                    .Build();

            _currentPlayers[unassignedIndexes[accompliceIndex]] = accomplicePlayer;
            accompliceRoles.Remove(accompliceRole);
            unassignedIndexes.Remove(unassignedIndexes[accompliceIndex]);

        }

        Console.WriteLine("Built Accomplice");
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.Role}");
        }

        // 3. Randomly assign 0 to 2 players the Bystander role
        int bystanderCount = random.Next(3);  // Random number between 0 and 2
        for (int i = 0; i < bystanderCount && unassignedIndexes.Count > 0; i++)
        {
            int bystanderIndex = random.Next(unassignedIndexes.Count);

            // PROTOTYPE
            Role bystanderPlayerRole = (Role)new Bystander().Clone();

            // BUILDER
            IPlayerBuilder citizenBuilder = roleFactory.GetCitizenBuilder(_currentPlayers[bystanderIndex].WebSocket);
            var bystanderPlayer = citizenBuilder.SetName(_currentPlayers[bystanderIndex].Name)
                                                .SetRole(bystanderPlayerRole)
                                                .SetAlive(true)
                                                .SetLoggedIn(_currentPlayers[bystanderIndex].IsLoggedIn)
                                                .SetHost(_currentPlayers[bystanderIndex].IsHost)
                                                .Build();

            _currentPlayers[unassignedIndexes[bystanderIndex]] = bystanderPlayer;
            unassignedIndexes.Remove(unassignedIndexes[bystanderIndex]);

        }

        Console.WriteLine("Built Bystander");
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.Role}");
        }

        Console.WriteLine("Unassigned indexes:");
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            Console.WriteLine(playerIndex);
        }

        // 4. Assign remaining players random roles from citizenRoles
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            // PROTOTYPE
            Role citizenRole = citizenRoles.Count > 0
                           ? (Role)citizenRoles[random.Next(citizenRoles.Count)].Clone()
                           : (Role)originalCitizenRoles[random.Next(originalCitizenRoles.Count)].Clone();
            // BUILDER
            IPlayerBuilder citizenBuilder = roleFactory.GetCitizenBuilder(_currentPlayers[playerIndex].WebSocket);
            var citizenPlayer = citizenBuilder.SetName(_currentPlayers[playerIndex].Name)
                                              .SetRole(citizenRole)
                                              .SetAlive(true)
                                              .SetLoggedIn(_currentPlayers[playerIndex].IsLoggedIn)
                                              .SetHost(_currentPlayers[playerIndex].IsHost)
                                              .Build();

            _currentPlayers[playerIndex] = citizenPlayer;
            citizenRoles.Remove(citizenRole);
        }

        Console.WriteLine("Built Citizen");
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.Role}");
        }

        Console.WriteLine("Final");

        // Notify each player of their assigned role
        foreach (var player in _currentPlayers)
        {
            Console.WriteLine($"{player.Name} | {player.RoleName}");
            await player.SendMessage(new CommandMessage
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
    
}

