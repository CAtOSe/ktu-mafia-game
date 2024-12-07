using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using System.Text;
using Mafia.Server.Models.Strategy;
using Mafia.Server.Models.Decorator;
using Mafia.Server.Models.Builder;
using System.Net.WebSockets;
using Mafia.Server.Controllers;
using Mafia.Server.Extensions;
using Mafia.Server.Logging;
using Mafia.Server.Models.Bridge;
using CommandMessage = Mafia.Server.Models.Messages.CommandMessage;
using Mafia.Server.Models.Adapter;
using Mafia.Server.Models.Flyweight;
using Mafia.Server.Models.GameConfigurationFactory;
using LogLevel = Mafia.Server.Logging.LogLevel;
using RoleFactorySelector = Mafia.Server.Models.AbstractFactory.RoleFactorySelector;
using Mafia.Server.Models.Iterator;
using Mafia.Server.Models.Iterator.ActionQueue;
using Mafia.Server.Models.State;
using Mafia.Server.Models.ChainOfResponsibility;
using Mafia.Server.Models.Mediator;
using Mafia.Server.Models.Memento;

namespace Mafia.Server.Services.GameService;

public class GameService : IGameService
{
    private readonly IChatService _chatService;
    private readonly TimeProvider _timeProvider;
    private readonly IChatServiceAdapter _chatAdapter;
    private readonly IGameConfigurationFactory _gameConfigurationFactory;
    private IGameConfiguration _gameConfiguration;
    private GameLogger _logger;
    private readonly IGameStateManager _stateManager;//STATE
    private readonly IChatMediator _chatMediator;
    private GameStateCaretaker _gameStateCaretaker = new(); // MEMENTO

    //private readonly GameController _gameController;


    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = true;

    private readonly List<Player> _currentPlayers = [];
    private List<ActionQueueEntry> _actionQueue = [];
    private List<ChatMessage> _dayStartAnnouncements = [];
    private List<Player> _playersWhoDiedInTheNight = [];
    private MorningAnnouncer _morningAnnouncer; // DECORATOR
    private DayEndHandler phaseHandler; // CHAIN OF RESPONSIBILITY

    private CancellationTokenSource _phaseCancelTokenSource; //  Token for canceling the phase cycle

    public List<Player> GetPlayers() => _currentPlayers;

    private DateTimeOffset _lastPhaseChange;
    private int _remainingPhaseTime;

    public bool IsPaused { get; private set; }
    public bool GameStarted { get; private set; }

    public GameService(
        IChatService chatService,
        TimeProvider timeProvider,
        IChatServiceAdapter chatAdapter,
        IGameConfigurationFactory gameConfigurationFactory,
        IGameStateManager stateManager,
        IChatMediator chatMediator)
        //GameController gameController)
    {
        IsPaused = false;
        _chatService = chatService;
        _timeProvider = timeProvider;
        _chatAdapter = chatAdapter;
        _gameConfigurationFactory = gameConfigurationFactory;
        _stateManager = stateManager;
        _chatMediator = chatMediator;
        //_gameController = gameController;
        _logger = GameLogger.Instance;
    }

    public void PauseTimer()
    {
        SaveGameState(); //MEMENTO Save game state before pausing
        IsPaused = true;
        _phaseCancelTokenSource.Cancel();
        var phaseTime = _isDayPhase ? _gameConfiguration.DayPhaseDuration : _gameConfiguration.NightPhaseDuration;
        _remainingPhaseTime = phaseTime - (int)(_timeProvider.GetUtcNow() - _lastPhaseChange).TotalMilliseconds;
        var updateMessage = new CommandMessage
        {
            Base = ResponseCommands.GameUpdate,
            Arguments = new List<string> { "paused", _remainingPhaseTime.ToString() }
        };
        NotifyAllPlayers(updateMessage);
    }

    public void ResumeTimer()
    {
        var memento = _gameStateCaretaker.RestoreState();
        if (memento != null)
        {
            RestoreGameState(memento); //MEMENTO Restore game state before resuming
        }
        else
        {
            _logger.Log(LogLevel.Error, "No saved game state to restore.");
        }
        
        IsPaused = false;
        var updateMessage = new CommandMessage
        {
            Base = ResponseCommands.GameUpdate,
            Arguments = new List<string> { "resumed", _remainingPhaseTime.ToString() }
        };
        NotifyAllPlayers(updateMessage);
        StartDayNightCycle(_remainingPhaseTime);
    }
    //MEMENTO
    public void SaveGameState()
    {
        var memento = new GameStateMemento(_currentPlayers, GameStarted, IsPaused);
        _gameStateCaretaker.SaveState(memento);
        _logger.Log(LogLevel.Debug, "Game state saved.");
    }
    public void RestoreGameState(GameStateMemento memento)
    {
        if (memento != null)
        {
            _currentPlayers.Clear();
            _currentPlayers.AddRange(memento.Players);
            GameStarted = memento.GameStarted;
            IsPaused = memento.IsPaused;
            
            // Printing game state info to console
            Console.WriteLine("\n===== Restored Game State =====");
            Console.WriteLine($"GameStarted: {GameStarted}");
            Console.WriteLine($"IsPaused: {IsPaused}");
            Console.WriteLine($"Total Players: {_currentPlayers.Count}");
        
            Console.WriteLine("\nPlayers Info:");
            foreach (var player in _currentPlayers)
            {
                Console.WriteLine($"Name: {player.Name}, Role: {player.RoleName}, Alive: {player.IsAlive}, Host: {player.IsHost}, Poisoned: {player.IsPoisoned}");
            }
            Console.WriteLine("================================\n");
            
            _logger.Log(LogLevel.Debug, "Game state restored from provided memento.");
        }
        else
        {
            Console.WriteLine("No game state to restore.");
            _logger.Log(LogLevel.Error, "No game state to restore from the provided memento.");
        }
    }

    /*public void PeekGameState()
    {
        var memento = _gameStateCaretaker.PeekState();
        if (memento != null)
        {
            _logger.Log(LogLevel.Debug, $"Peeked Game State: Players: {memento.Players.Count}, GameStarted: {memento.GameStarted}, IsPaused: {memento.IsPaused}");
        }
        else
        {
            _logger.Log(LogLevel.Error, "No game state to peek.");
        }
    }*/

    public async Task DisconnectPlayer(WebSocket webSocket)
    {
        var player = GetByConnection(webSocket);
        if (player is null) return;

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

    public async Task TryAddPlayer(WebSocket webSocket, string username)
    {
        var existingPlayer = GetByConnection(webSocket);
        if (existingPlayer is not null && existingPlayer.IsLoggedIn)
        {
            await webSocket.SendMessage(new CommandMessage
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.AlreadyLoggedIn,
            }.ToString());
            return;
        }

        var usernameTaken = _currentPlayers.Any(p =>
            p.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (usernameTaken)
        {
            await webSocket.SendMessage(new CommandMessage
            {
                Base = ResponseCommands.Error,
                Error = ErrorMessages.UsernameTaken
            }.ToString());
            return;
        }

        var playerBuilder = new PlayerBuilder(webSocket);
        playerBuilder.SetName(username);
        playerBuilder.SetHost(_currentPlayers.Count == 0);
        playerBuilder.SetAlive(true);
        var player = playerBuilder.Build();

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
            _logger.Log("Game already started");
            return;
        }

        if (_currentPlayers.Count < 3)
        {
            throw new InvalidOperationException("There must be at least 3 players to start the game.");
        }

        _gameConfiguration = _gameConfigurationFactory.GetGameConfiguration(difficultyLevel);

        _logger.Log("Assigning roles and starting the countdown.");
        await NotifyAllPlayers(new CommandMessage
        {
            Base = ResponseCommands.StartCountdown,
            Arguments = [_gameConfiguration.BeginCountdown.ToString()]
        });
        var gameStartTask = Task.Delay(_gameConfiguration.BeginCountdown).ContinueWith(async t =>
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
            _logger.Log(LogLevel.Error, "Invalid action: Either the user or the target player does not exist.");
            return;
        }

        _logger.Log(LogLevel.PlayerAction, "Received NIGHT ACTION: " + actionUser.Name + " " + actionUser.Role + ", " + targetPlayer.Name);

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
            await _chatMediator.SendMessage("", "You have chosen " + targetPlayer.Name, actionUser.Name, "nightAction");
        }
        else
        {
            await _chatMediator.SendMessage("", "You have canceled your selection", actionUser.Name, "nightAction");
        }
    }

    public async Task VoteFor(Player player, string username)
    {
        // Find the target player by username
        var targetPlayer = _currentPlayers.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));

        // Check if it's the day phase and if the target is not null
        if (!_isDayPhase || targetPlayer is null) return;

        // Use the Bridge pattern - set the action executor based on player state
        IRoleActionExecutor actionExecutor = player.IsPoisoned
            ? new PoisonedActionExecutor(player.Role.RoleAlgorithmPoisoned)
            : new StandardActionExecutor(player.Role.RoleAlgorithm);

        // Assign the action executor to the role
        player.Role.SetActionExecutor(actionExecutor);

        // Execute the voting action with the assigned executor
        await player.Role.ExecuteAction(player, targetPlayer, null, new List<ChatMessage>());

        // Send appropriate message based on vote selection or cancellation
        if (player.CurrentVote != targetPlayer)
        {
            await _chatMediator.SendMessage("", $"You have chosen {targetPlayer.Name}", player.Name, "dayAction");
            player.CurrentVote = targetPlayer;
        }
        else
        {
            await _chatMediator.SendMessage("", "You have canceled your selection", player.Name, "dayAction");
            player.CurrentVote = null;
        }
    }
    //BEFORE BRIDGE
    /*public async Task VoteFor(Player player, string username)
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
    }*/

    private async Task ExecuteNightActions()
    {
        // Create Concrete Aggregator
        var actionQueue = new ActionQueue(_actionQueue);

        // Get the iterator from the ActionQueue
        var iterator = actionQueue.CreateIterator();

        // Initialize the context for night actions
        var context = new RoleActionContext
        {
            Players = _currentPlayers,
            QueuedActions = _actionQueue,
        };

        List<ChatMessage> nightMessages = new List<ChatMessage>();

        // Dictionary to track players' alive status before the actions
        Dictionary<Player, bool> initialAliveStatus = new Dictionary<Player, bool>();

        foreach (var player in _currentPlayers)
        {
            player.IsPoisoned = false;
            initialAliveStatus[player] = player.IsAlive;
        }

        // Start executing night actions in the queue, using Iterator
        for (var nightAction = iterator.First(); nightAction != null; nightAction = iterator.Next()) // Iterator
        {
            var user = nightAction.User;
            var target = nightAction.Target;

            // Use the Bridge pattern - set the executor based on the user's state
            IRoleActionExecutor actionExecutor = user.IsPoisoned
                ? new PoisonedActionExecutor(user.Role.RoleAlgorithmPoisoned)
                : new StandardActionExecutor(user.Role.RoleAlgorithm);

            // Assign the action executor to the role
            user.Role.SetActionExecutor(actionExecutor);

            // Execute the action logic through the assigned executor, uses TEMPLATE METHOD
            await user.Role.ExecuteAction(user, target, context, nightMessages);
        }

        // Send messages about night actions (e.g., notifications to the chat)
        foreach (var nightMessage in nightMessages)
        {
            await _chatMediator.SendMessage(nightMessage);
        }

        // Clear the action queue after all actions have been executed
        _actionQueue.Clear();

        // Day Start Announcements
        int deathInNightCount = 0;
        foreach (var player in _currentPlayers.Where(player => initialAliveStatus[player] && !player.IsAlive))
        {
            deathInNightCount++;
            var deathMessage = new ChatMessage("", "You died.", player.Name, "nightNotification");
            _playersWhoDiedInTheNight.Add(player);
            await _chatMediator.SendMessage(deathMessage);
        }
    }
    /*if(deathInNightCount == 0)
    {
        var dayAnnouncement = new ChatMessage("","No one has died in the night.", "everyone", "dayNotification");
        _dayStartAnnouncements.Add(dayAnnouncement);
    }
}

//BEFORE BRIDGE
/*private async Task ExecuteNightActions()
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
    }

    await SendAlivePlayerList();

    // Clear the nightActions list after processing all actions
    _actionQueue.Clear();
}*/

    private async Task ExecuteDayActions()
    {

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
        await _chatMediator.SendMessage(finalVotesMessage);

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
            foreach (var votingResultMessage in votingResultsMessages)
            {
                await _chatMediator.SendMessage(votingResultMessage);
            }
            var votingResultPersonalMessage = new ChatMessage("", "You died.", votedOff.Name, "dayNotification");
            await _chatMediator.SendMessage(votingResultPersonalMessage);
        }
        else // More than one max value
        {
            var votingResultMessage = new ChatMessage("", "No player has been voted of by the town today. (Tie)", "everyone", "dayNotification");
            await _chatMediator.SendMessage(votingResultMessage);
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
        _gameStateCaretaker = new GameStateCaretaker(); // Clear saved states
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
                await _chatMediator.SendMessage("", phaseName + " " + _phaseCounter, "everyone",
                    chatMessageType); // DAY 1 / NIGHT 1
                await UpdateDayNightPhase();
                await AnnounceNightDeaths();
            }
            else
            {
                var totalPhaseTime = _isDayPhase ?
                    _gameConfiguration.DayPhaseDuration :
                    _gameConfiguration.NightPhaseDuration;
                var elapsedPhaseTime = totalPhaseTime - remainingPhaseTime;
                _lastPhaseChange = _timeProvider.GetUtcNow().Subtract(TimeSpan.FromMilliseconds(elapsedPhaseTime));
            }

            try
            {
                if (_isDayPhase)
                {
                    await Task.Delay(remainingPhaseTime == 0 ? _gameConfiguration.DayPhaseDuration : remainingPhaseTime,
                        token);
                    await ExecuteDayActions();
                }
                else
                {
                    await Task.Delay(
                        remainingPhaseTime == 0 ? _gameConfiguration.NightPhaseDuration : remainingPhaseTime, token);
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

    private async Task AnnounceNightDeaths()
    {
        HandlerContext handlerContext = new HandlerContext(_isDayPhase, _phaseCounter, _currentPlayers, _playersWhoDiedInTheNight,
            _dayStartAnnouncements, _morningAnnouncer, _chatAdapter);
        await phaseHandler.HandleRequest(handlerContext);

        /*
        if (_isDayPhase)
        {
            if (_phaseCounter != 1) // Not on the first day
            {
                _morningAnnouncer.DayStartAnnouncements(_currentPlayers, _playersWhoDiedInTheNight, _dayStartAnnouncements); // DECORATOR
                //Sending "Player 1 has died in the night."
                foreach (ChatMessage announcement in _dayStartAnnouncements)
                {
                    await _chatAdapter.SendMessage(announcement);
                }
                _dayStartAnnouncements.Clear();
                _playersWhoDiedInTheNight.Clear();
            }
            else // On the first day, notify evil players of all the evil team members.
            {
                List<ChatMessage> evilTeamMessages = new List<ChatMessage>();
                // Create Concrete Aggregator
                var evilPlayersList = new EvilPlayerList(_currentPlayers);

                // Get the iterator from the ActionQueue
                var recipientsIterator = evilPlayersList.CreateIterator();
                var evilTeamIterator = evilPlayersList.CreateIterator();

                for (var evilPlayerToSendTo = recipientsIterator.First(); evilPlayerToSendTo != null; evilPlayerToSendTo = recipientsIterator.Next()) // Iterator
                {
                    evilTeamMessages.Add(new ChatMessage("", "Your evil team consists of these players:", evilPlayerToSendTo.Name, "dayNotification"));

                    for (var evilPlayerOnTeam = evilTeamIterator.First(); evilPlayerOnTeam != null; evilPlayerOnTeam = evilTeamIterator.Next()) // Iterator
                    {
                        evilTeamMessages.Add(new ChatMessage("", evilPlayerOnTeam.Name + " is " + evilPlayerOnTeam.RoleName, evilPlayerToSendTo.Name, "dayNotification"));
                    }
                }
                //evilTeamMessages.Add(new ChatMessage("", "Your evil team consists of these players:", ))

                foreach (ChatMessage announcement in evilTeamMessages)
                {
                    await _chatAdapter.SendMessage(announcement);
                }
            }
        }*/
    }

    private async Task UpdateDayNightPhase()
    {
        await SendAlivePlayerList();

        var phaseName = _isDayPhase ? "day" : "night";
        var timeoutDuration = _isDayPhase ? _gameConfiguration.DayPhaseDuration : _gameConfiguration.NightPhaseDuration;
        _logger.Log(LogLevel.CycleUpdate, $"New phase: {phaseName} {_phaseCounter}");
        await NotifyAllPlayers(new CommandMessage
        {
            Base = ResponseCommands.PhaseChange,
            Arguments = [phaseName, timeoutDuration.ToString(), _phaseCounter.ToString()]
        });
        string winnerTeam = DidAnyTeamWin();
        if (winnerTeam != null)
        {
            _stateManager.EndGame(); //STATE
            //_gameController.EndGame();
            _logger.Log(winnerTeam + " team has won the game!");
            await _chatMediator.SendMessage("", winnerTeam + " team has won the game!", "everyone", "server");
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
        var roleFactorySelector = new RoleFactorySelector();
        var roleFactory = roleFactorySelector.FactoryMethod(preset);

        // ABSTRACT FACTORY
        var killerRoles = roleFactory.GetKillerRoles();
        var accompliceRoles = roleFactory.GetAccompliceRoles();
        var citizenRoles = roleFactory.GetCitizenRoles();
        // DECORATOR
        _morningAnnouncer = roleFactory.GetAnnouncer();
        // CHAIN OF RESPONSIBILITY
        phaseHandler = new DayEndHandler();
        var dayStartHandler = new DayStartHandler();
        var firstDayStartHandler = new FirstDayStartHandler();

        phaseHandler.SetNext(dayStartHandler);  // DayEndHandler -> DayStartHandler
        dayStartHandler.SetNext(firstDayStartHandler); // DayStartHandler -> FirstDayStartHandler


        // PROTOTYPE
        var originalCitizenRoles = new List<Role>(citizenRoles);

        var accompliceCount = GetAccompliceCount(_currentPlayers.Count);

        var random = new Random();

        // Create a list to track unassigned players by index
        var unassignedIndexes = Enumerable.Range(0, _currentPlayers.Count).ToList();

        // 1. Assign a random Killer role to one player
        var killerIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];

        // PROTOTYPE
        var killerRole = killerRoles[random.Next(killerRoles.Count)];

        // BUILDER
        var killerBuilder = new PlayerBuilder(_currentPlayers[killerIndex].WebSocket);
        var killerPlayer = killerBuilder.SetName(_currentPlayers[killerIndex].Name)
                                        .SetRole(killerRole)
                                        .SetAlive(true)
                                        .SetHost(_currentPlayers[killerIndex].IsHost)
                                        .Build();

        _currentPlayers[killerIndex] = killerPlayer;

        // Remove the assigned killer role and the player index from the tracking list
        killerRoles.Remove(killerPlayer.Role);
        unassignedIndexes.Remove(killerIndex);

        _logger.Log(LogLevel.Debug, "Built Killer");
        foreach (var player in _currentPlayers)
        {
            _logger.Log(LogLevel.Debug, $"{player.Name} | {player.Role}");
        }


        // 2. Assign accomplice roles
        for (var i = 0; i < accompliceCount; i++)
        {
            if (unassignedIndexes.Count == 0) break;  // Safety check: stop if no players are left to assign

            var accompliceIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];

            // PROTOTYPE
            var accompliceRole = accompliceRoles.Count > 0
                                    ? (Role)accompliceRoles[random.Next(accompliceRoles.Count)].Clone()
                                    : (Role)new Lackey().Clone();
            // BUILDER
            var accompliceBuilder = new PlayerBuilder(_currentPlayers[accompliceIndex].WebSocket);
            var accomplicePlayer = accompliceBuilder.SetName(_currentPlayers[accompliceIndex].Name)
                                                    .SetRole(accompliceRole)
                                                    .SetAlive(true)
                                                    .SetHost(_currentPlayers[accompliceIndex].IsHost)
                                                    .Build();

            _currentPlayers[accompliceIndex] = accomplicePlayer;
            accompliceRoles.Remove(accompliceRole);
            unassignedIndexes.Remove(accompliceIndex);

        }

        _logger.Log(LogLevel.Debug, "Built Accomplice");
        foreach (var player in _currentPlayers)
        {
            _logger.Log(LogLevel.Debug, $"{player.Name} | {player.Role}");
        }

        // 3. Randomly assign 0 to 2 players the Bystander role
        var bystanderCount = random.Next(3);  // Random number between 0 and 2
        for (var i = 0; i < bystanderCount && unassignedIndexes.Count > 0; i++)
        {
            var bystanderIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];

            // PROTOTYPE
            var bystanderPlayerRole = (Role)new Bystander().Clone(); // Shallow copy

            /*
            Role bystanderPlayerRoleCopy = new Bystander().DeepCopy();// Deep copy
            Console.WriteLine("Shallow BYSTANDER copy: " + bystanderPlayerRole.GetHashCode().ToString());
            Console.WriteLine("Deep BYSTANDER copy: " + bystanderPlayerRoleCopy.GetHashCode().ToString());
            Console.WriteLine("Is same reference: " + Object.ReferenceEquals(bystanderPlayerRole, bystanderPlayerRoleCopy));
            */

            // BUILDER
            var citizenBuilder = new PlayerBuilder(_currentPlayers[bystanderIndex].WebSocket);
            var bystanderPlayer = citizenBuilder.SetName(_currentPlayers[bystanderIndex].Name)
                                                .SetRole(bystanderPlayerRole)
                                                .SetAlive(true)
                                                .SetHost(_currentPlayers[bystanderIndex].IsHost)
                                                .Build();

            _currentPlayers[bystanderIndex] = bystanderPlayer;
            unassignedIndexes.Remove(bystanderIndex);
        }

        _logger.Log(LogLevel.Debug, "Built Bystander");
        foreach (var player in _currentPlayers)
        {
            _logger.Log(LogLevel.Debug, $"{player.Name} | {player.Role}");
        }

        _logger.Log(LogLevel.Debug, "Unassigned indexes:");
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            _logger.Log(LogLevel.Debug, $"{playerIndex}");
        }

        // 4. Assign remaining players random roles from citizenRoles
        foreach (var playerIndex in unassignedIndexes.ToList())  // Iterate over unassigned players
        {
            // PROTOTYPE
            var citizenRole = citizenRoles.Count > 0
                           ? (Role)citizenRoles[random.Next(citizenRoles.Count)].Clone()
                           : (Role)originalCitizenRoles[random.Next(originalCitizenRoles.Count)].Clone();
            // BUILDER
            var citizenBuilder = new PlayerBuilder(_currentPlayers[playerIndex].WebSocket);
            var citizenPlayer = citizenBuilder.SetName(_currentPlayers[playerIndex].Name)
                                              .SetRole(citizenRole)
                                              .SetAlive(true)
                                              .SetHost(_currentPlayers[playerIndex].IsHost)
                                              .Build();

            _currentPlayers[playerIndex] = citizenPlayer;
            citizenRoles.Remove(citizenRole);
        }

        _logger.Log(LogLevel.Debug, "Built Citizen");
        foreach (var player in _currentPlayers)
        {
            _logger.Log(LogLevel.Debug, $"{player.Name} | {player.Role}");
        }

        _logger.Log(LogLevel.Debug, "Finished building roles");
        // ADDED FLYWEIGHT
        // Notify each player of their assigned role and image
        foreach (var player in _currentPlayers)
        {
            var roleImage = RoleImageFactory.GetRoleImage(player.RoleName);

            // Retrieve the image path as a string
            var roleImagePath = roleImage.GetImagePath();

            // Notify each player of their assigned role, including the image path
            await player.SendMessage(new CommandMessage
            {
                Base = ResponseCommands.RoleAssigned,
                Arguments = new string[]
                {
                    player.RoleName,
                    player.Role.Alignment,
                    player.Role.Ability,
                    player.Role.Goal,
                    roleImagePath // Includes the image path
                }
            });
        }
    }

    private Player GetByConnection(WebSocket webSocket) =>
        _currentPlayers.FirstOrDefault(x => x.WebSocket == webSocket);

    private int GetAccompliceCount(int playerCount)
    {
        if (playerCount <= 5) return 0;
        if (playerCount >= 6 && playerCount <= 9) return 1;
        if (playerCount >= 10 && playerCount <= 12) return 2;
        if (playerCount >= 13) return 3;

        return 0;
    }

}

