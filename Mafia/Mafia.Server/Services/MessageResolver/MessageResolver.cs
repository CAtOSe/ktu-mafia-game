using Mafia.Server.Models;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolver(IGameService gameService) : IMessageResolver
{
    public async Task HandleMessage(Player player, string message)
    {
        var command = Message.FromString(message);

        switch (command.Base)
        {
            case BaseCommands.Login when command.Arguments.Count == 1:
                var username = command.Arguments[0];
                await gameService.TryAddPlayer(player, username);
                return;
            case BaseCommands.Disconnect:
                await gameService.DisconnectPlayer(player);
                return;
        }
        if (message.Equals(Messages.Login))
        {
            gameService.AddNewPlayer(player);
        }
        
        if (message == "start-game")
        {
            // Call the StartGame method in the game service
            gameService.StartGame();

            // Notify all players that the game has started
            foreach (var p in gameService.GetPlayers())
            {
                await p.SendMessage("game-started");
            }
        }
        if (message == "get-roles")
        {
            var roles = gameService.GetPlayerRoles();
            var rolesMessage = string.Join(",", roles.Select(r => $"{r.Key}:{r.Value}"));
            await player.SendMessage($"roles-list:{rolesMessage}");
        }

        
    }
}