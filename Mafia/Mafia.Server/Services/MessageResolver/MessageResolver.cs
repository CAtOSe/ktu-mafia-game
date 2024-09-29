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
            case RequestCommands.Login when command.Arguments.Count == 1:
                var username = command.Arguments[0];
                await gameService.TryAddPlayer(player, username);
                return;
            case RequestCommands.Disconnect:
                await gameService.DisconnectPlayer(player);
                return;
            case RequestCommands.StartGame:
                await gameService.StartGame();
                return;
        }
        /*
        if (message == "get-roles")
        {
            var roles = gameService.GetPlayerRoles();
            var rolesMessage = string.Join(",", roles.Select(r => $"{r.Key}:{r.Value}"));
            await player.SendMessage($"roles-list:{rolesMessage}");
        }
*/
        
    }
}