using Mafia.Server.Models;
using Mafia.Server.Models.Commands;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolver(IGameService gameService, IChatService chatService) : IMessageResolver
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
            case RequestCommands.NightAction when command.Arguments.Count == 2:
                var actionTarget = command.Arguments[0];
                var actionType = command.Arguments[1];
                await gameService.NightAction(player, actionTarget, actionType);
                return;
            case RequestCommands.Chat:
                await chatService.HandleIncomingMessage(player, command);
                return;
        }
    }
}