using Mafia.Server.Models;
using Mafia.Server.Models.Messages;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolver(IGameService gameService, IChatService chatService) : IMessageResolver
{
    public async Task HandleMessage(Player player, string message)
    {
        var command = CommandMessage.FromString(message);
        switch (command.Base)
        {
            case RequestCommands.Login when command.Arguments.Count == 1:
            {
                var username = command.Arguments[0];
                await gameService.TryAddPlayer(player, username);
                return;
            }
            case RequestCommands.Disconnect:
            {
                await gameService.DisconnectPlayer(player);
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
            case RequestCommands.Chat when command.Arguments.Count == 3:
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
    public async Task SendGameUpdate(string status, int remainingTime)
    {
        var updateMessage = new CommandMessage
        {
            Base = ResponseCommands.GameUpdate, // Pridėkite `GameUpdate` kaip naują atsakymo tipą
            Arguments = new List<string> { status, remainingTime.ToString() }
        };

        // Siųskite visiems prijungtiems žaidėjams per `chatService` arba tiesiogiai
        await chatService.SendChatMessage("", updateMessage.ToString(), "everyone", "gameUpdate");
    }
}