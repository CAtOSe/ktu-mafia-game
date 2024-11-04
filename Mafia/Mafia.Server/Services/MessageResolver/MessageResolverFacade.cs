using System.Net.WebSockets;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolverFacade(IGameService gameService, IChatService chatService) : IMessageResolverFacade
{
    public async Task HandleMessage(WebSocket webSocket, string message)
    {
        var player = gameService.GetPlayers().FirstOrDefault(x => x.WebSocket == webSocket);
        var command = CommandMessage.FromString(message);
        switch (command.Base)
        {
            case RequestCommands.Login when command.Arguments.Count == 1:
            {
                var username = command.Arguments[0];
                await gameService.TryAddPlayer(webSocket, username);
                return;
            }
            case RequestCommands.Disconnect:
            {
                await gameService.DisconnectPlayer(webSocket);
                return;
            }
            case RequestCommands.StartGame when command.Arguments.Count == 1:
            {
                var difficultyLevel = command.Arguments[0];
                await gameService.StartGame(difficultyLevel);
                return;
            }
            case RequestCommands.NightAction when command.Arguments.Count == 2:
            {
                var actionTarget = command.Arguments[0];
                var actionType = command.Arguments[1];
                await gameService.AddNightActionToList(player, actionTarget, actionType);
                return;
            }
            case RequestCommands.Chat when command.Arguments.Count == 3 && player is not null:
            {
                var content = command.Arguments[0];
                var recipient = command.Arguments[1];
                var category = command.Arguments[2];
                await chatService.SendChatMessage(player.Name, content, recipient, category);
                return;
            }
            case RequestCommands.Vote when command.Arguments.Count > 0:
            {
                var username = command.Arguments[0];
                await gameService.VoteFor(player, username);
                return;
            }
        }
    }
}