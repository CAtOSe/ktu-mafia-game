using Mafia.Server.Command;

namespace Mafia.Server.Models.Facade;

public class GameRoleFacade
{
    private readonly RoleFactorySelector _roleFactorySelector;
    private readonly ICommand _pauseResumeCommand;

    public GameRoleFacade(RoleFactorySelector roleFactorySelector, ICommand pauseResumeCommand)
    {
        _roleFactorySelector = roleFactorySelector;
        _pauseResumeCommand = pauseResumeCommand;
    }

    // Method to assign a role to a player
    public PlayerRole AssignRoleToPlayer(string playerId, string roleType)
    {
        var roleFactory = _roleFactorySelector.SelectFactory(roleType);
        var playerRole = roleFactory.CreateRole(roleType);
        // Additional logic to assign the role to the player in the system
        Console.WriteLine($"Assigned role {playerRole} to player {playerId}");
        return playerRole;
    }

    // Method to execute a command (such as pause or resume the game)
    public void ExecuteCommand(string commandType)
    {
        if (commandType == "Pause" || commandType == "Resume")
        {
            _pauseResumeCommand.Execute();
            Console.WriteLine($"{commandType} command executed.");
        }
        else
        {
            Console.WriteLine($"Command {commandType} is not supported by GameRoleFacade.");
        }
    }

    // Method to initiate a role-based action
    public void PerformRoleAction(string playerId, string actionType)
    {
        // Use playerId to fetch player details, role, etc.
        // This example assumes a PlayerRole with an action method or an Action interface
        Console.WriteLine($"Performing {actionType} for player {playerId}");
    }
}
