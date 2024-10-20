﻿using Mafia.Server.Models;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using System.Text;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Services.GameService;

public class GameService(IChatService chatService) : IGameService
{

    private readonly List<Player> _currentPlayers = [];
    
    private CancellationTokenSource _cancellationTokenSource; //  Token for canceling the phase cycle
    private bool GameStarted { get; set; } = false;

    private volatile int _phaseCounter = 1;
    private volatile bool _isDayPhase = true;

    private List<ActionQueueEntry> _actionQueue = [];
    private List<ChatMessage> _dayStartAnnouncements = [];

    public List<Player> GetPlayers() => _currentPlayers;

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
        chatService.SetPlayers(_currentPlayers);
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
        //await AssignItems();

        await gameStartTask;
        GameStarted = true;
        StartDayNightCycle();
    }

    public async Task AddNightActionToList(Player actionUser, string actionTarget, string actionType)
    {
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
            await chatService.SendChatMessage("","You have chosen " + targetPlayer.Name, actionUser.Name, "nightAction");
        }
        else
        {
            await chatService.SendChatMessage("", "You have canceled your selection", actionUser.Name, "nightAction");
        }
    }

    public async Task VoteFor(Player player, string username)
    {
        var targetPlayer =
            _currentPlayers.FirstOrDefault(x => x.Name.Equals(username, StringComparison.OrdinalIgnoreCase));
        if (!_isDayPhase || targetPlayer is null) return;

        if (player.CurrentVote != targetPlayer)
        { 
            await chatService.SendChatMessage("","You have chosen " + targetPlayer.Name, player.Name, "dayAction"); 
            player.CurrentVote = targetPlayer;
        }
        else
        {
            await chatService.SendChatMessage("", "You have canceled your selection", player.Name, "dayAction");
            player.CurrentVote = null;
        }
    }

    private async Task ExecuteNightActions()
    {
        // Sort nightActions by actionType based on the custom order
        //nightActions = nightActions.OrderBy(action => actionOrder.IndexOf(action.Target.RoleName)).ToList();

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
            await chatService.SendChatMessage(nightMessage);
        }

        int deathInNightCount = 0;
        foreach (var player in _currentPlayers)
        {
            // Check if the player died (alive status changed from true to false)
            if (initialAliveStatus[player] && !player.IsAlive)
            {
                deathInNightCount++;
                var deathMessage = new ChatMessage("", "You died.", player.Name, "nightNotification");
                var dayAnnouncement = new ChatMessage("", player.Name + " has died in the night.", "everyone", "dayNotification");
                _dayStartAnnouncements.Add(dayAnnouncement);
                await chatService.SendChatMessage(deathMessage);
            }
        }
        if(deathInNightCount == 0)
        {
            var dayAnnouncement = new ChatMessage("","No one has died in the night.", "everyone", "dayNotification");
            _dayStartAnnouncements.Add(dayAnnouncement);
        }

        await SendAlivePlayerList();

        // Clear the nightActions list after processing all actions
        _actionQueue.Clear();
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
        await chatService.SendChatMessage(finalVotesMessage);

        if (votes.Count == 0) // No players voted
        {
            return;
        }
        
        if (votes.Count == 1 || votes[1].Value != votes[0].Value) // One max value
        {
            var votedOff = votes[0].Key;
            votedOff.IsAlive = false;
            await SendAlivePlayerList();
            var votingResultMessage = new ChatMessage("", votedOff.Name + " has been voted off by the town.", "everyone", "dayNotification");
            await chatService.SendChatMessage(votingResultMessage);
            var votingResultPersonalMessage = new ChatMessage("", "You died.", votedOff.Name, "dayNotification");
            await chatService.SendChatMessage(votingResultPersonalMessage);
        }
        else // More than one max value
        {
            var votingResultMessage = new ChatMessage("", "No player has been voted of by the town today. (Tie)", "everyone", "dayNotification");
            await chatService.SendChatMessage(votingResultMessage);
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
    
    private Task NotifyAllPlayers(CommandMessage commandMessage)
    {
        var notifications = _currentPlayers.Select(p => p.SendMessage(commandMessage));
        return Task.WhenAll(notifications);
    }

    private void ResetGame()
    {
        GameStarted = false;
        _phaseCounter = 1;
        chatService.ResetChat();
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
            await chatService.SendChatMessage("", phaseName + " " + _phaseCounter, "everyone", chatMessageType);

            await UpdateDayNightPhase();
            
            while (GameStarted && !token.IsCancellationRequested) 
            {
                if (_isDayPhase)
                {
                    await Task.Delay(GameConfiguration.DayPhaseDuration, token);
                }
                else
                {
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
                //Sending "DAY 1" / "NIGHT 1"/
                chatMessageType = _isDayPhase ? "dayStart" : "nightStart";
                phaseName = _isDayPhase ? "DAY" : "NIGHT";
                await chatService.SendChatMessage("", phaseName + " " + _phaseCounter, "everyone", chatMessageType);

                //Sending "Player 1 has died in the night."
                foreach(ChatMessage announcement in _dayStartAnnouncements)
                {
                    await chatService.SendChatMessage(announcement);
                }
                _dayStartAnnouncements.Clear();

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
        await NotifyAllPlayers(new CommandMessage
        {
            Base = ResponseCommands.PhaseChange,
            Arguments = [phaseName, timeoutDuration.ToString(), _phaseCounter.ToString()]
        });
        string winnerTeam = DidAnyTeamWin();
        if (winnerTeam != null)
        {
            Console.WriteLine(winnerTeam + " team has won the game!");
            await chatService.SendChatMessage("", winnerTeam + " team has won the game!", "everyone", "server");
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
    private async Task AssignRoles(string preset)
    {
        RoleFactorySelector roleFactorySelector = new RoleFactorySelector();
        RoleFactory roleFactory = roleFactorySelector.FactoryMethod(preset);

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
        int killerIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];
        var killerRole = killerRoles[random.Next(killerRoles.Count)];
        
        _currentPlayers[killerIndex].Role = killerRole;

        // Remove the assigned killer role and the player index from the tracking list
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

            int accompliceIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];
            Role accompliceRole = accompliceRoles.Count > 0
                                    ? accompliceRoles[random.Next(accompliceRoles.Count)]
                                    : new Lackey();

            // Builder
            _currentPlayers[accompliceIndex].Role = accompliceRole;
            accompliceRoles.Remove(accompliceRole);
            unassignedIndexes.Remove(accompliceIndex);
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
            int bystanderIndex = unassignedIndexes[random.Next(unassignedIndexes.Count)];
            _currentPlayers[bystanderIndex].Role = new Bystander();
            unassignedIndexes.Remove(bystanderIndex);
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
            Role citizenRole = citizenRoles.Count > 0
                                ? citizenRoles[random.Next(citizenRoles.Count)]
                                : originalCitizenRoles[random.Next(originalCitizenRoles.Count)];
            _currentPlayers[playerIndex].Role = citizenRole;
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

