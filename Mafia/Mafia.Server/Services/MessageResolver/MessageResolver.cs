using Mafia.Server.Models;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Services.MessageResolver;

public class MessageResolver(IGameService gameService) : IMessageResolver
{
    public async Task HandleMessage(Player player, string message)
    {
        // TODO: Create a better message resolver/matcher
        if (message.Equals(Messages.Login))
        {
            gameService.AddNewPlayer(player);
        }
    }
}