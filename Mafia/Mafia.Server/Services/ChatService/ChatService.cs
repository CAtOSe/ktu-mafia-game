using Mafia.Server.Models;
using Mafia.Server.Services.GameService;
using Mafia.Server.Models.Commands;


namespace Mafia.Server.Services.ChatService;

public class ChatService(IGameService gameService) : IChatService
{
    public async Task HandleIncomingMessage(Player player, Message message)
    {
        var alivePlayers = GetAlivePlayers();
        
        var chatMessage = new Message
        {
            Base = ResponseCommands.Chat,
            Arguments = new List<string> { player.Name, message.Arguments[0] } 
        };
        
        await NotifyPlayers(alivePlayers, chatMessage);
    }

    private List<Player> GetAlivePlayers() => gameService.GetPlayers()
        .Where(x => x.IsAlive)
        .ToList();
    
    private async Task NotifyPlayers(List<Player> players, Message message)
    {
        var notifications = players.Select(p => p.SendMessage(message));
        await Task.WhenAll(notifications);
    }
}
