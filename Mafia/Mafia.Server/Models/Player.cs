using System.Net.WebSockets;
using Mafia.Server.Extensions;
using System.Net.WebSockets;

namespace Mafia.Server.Models;

//public class Player(WebSocket webSocket)

public class Player
{
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;
    public string Name { get; set; }

    public string Role { get; set; } = "None"; //Default role set to None

    private WebSocket webSocket;

    public Player(WebSocket webSocket)
    {
        this.webSocket = webSocket;
        Name = "Guest"; // if error occurs, player will be displayed as guest
    }
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
