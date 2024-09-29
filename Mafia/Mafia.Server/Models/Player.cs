using System.Net.WebSockets;
using Mafia.Server.Extensions;
using System.Net.WebSockets;
using Mafia.Server.Models.Commands;

namespace Mafia.Server.Models;

public class Player(WebSocket webSocket)
{
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;
    public string Name { get; set; } = "Guest";

    public string Role { get; set; } = "None"; //Default role set to None

    private WebSocket webSocket;

    public async Task SendMessage(Message message)
    {
        await webSocket.SendMessage(message.ToString());
    }

    public async void CloseConnection()
    {
        if (webSocket.State != WebSocketState.Open) return;
        
        await webSocket.SendMessage(BaseCommands.Disconnect);
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
            "GameService:CloseConnection",
            CancellationToken.None);
    }
}
