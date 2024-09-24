using Mafia.Server.Models;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolver(IGameService gameService) : IMessageResolver
{
    public async Task HandleMessage(Player player, string message)
    {
        // TODO: Create a better message resolver/matcher
        /*if (message.Equals(Messages.Login))
        {
            gameService.AddNewPlayer(player);
        }*/
        
        if (message.StartsWith("login:"))
        {
            var username = message.Substring("login:".Length);
            if (await gameService.IsUsernameAvailable(username))
            {
                player.Name = username;
                await player.SendMessage("logged-in"); // Notify the player of successful login
                await gameService.AddPlayer(player); // Add the player to the game
            }
            else
            {
                await player.SendMessage("error: Username is already taken."); // Notify of failure
            }
        }
        if (message.Equals(Messages.Login))
        {
            gameService.AddNewPlayer(player);
        }
        else if (message.Equals(Messages.StartGame))
        {
            gameService.StartGame();
        }
        
    }
}