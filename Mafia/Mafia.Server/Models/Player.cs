using System.Net.WebSockets;
using Mafia.Server.Extensions;

namespace Mafia.Server.Models;

public class Player(WebSocket webSocket)
{
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;

    public async Task SendMessage(string message)
    {
        await webSocket.SendMessage(message);
    }

    public async void CloseConnection()
    {
        if (webSocket.State != WebSocketState.Open) return;
        
        await webSocket.SendMessage(Messages.Disconnect);
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
            "GameService:CloseConnection",
            CancellationToken.None);
    }
}
