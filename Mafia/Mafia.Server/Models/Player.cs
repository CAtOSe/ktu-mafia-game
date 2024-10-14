using System.Net.WebSockets;
using Mafia.Server.Extensions;
using System.Net.WebSockets;
using Mafia.Server.Models.Commands;
using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models;

public class Player(WebSocket webSocket)
{
    public DateTime CreationTime { get; init; } = DateTime.UtcNow;
    public string Name { get; set; } = "Guest";
    public Role Role { get; set; } = null;
    public string RoleName => Role.Name;
    public string RoleType => Role.RoleType;
    public bool IsLoggedIn { get; set; } = false;
    public bool IsHost { get; set; } = false;
    public bool IsAlive { get; set; } = true;
    public List<Item> Inventory { get; set; } = new();

    public async Task SendMessage(Message message)
    {
        await webSocket.SendMessage(message.ToString());
    }

    public async void CloseConnection()
    {
        if (webSocket.State != WebSocketState.Open) return;
        
        Console.WriteLine("Connection closed.");
        await webSocket.SendMessage(RequestCommands.Disconnect);
        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, 
            "GameService:CloseConnection",
            CancellationToken.None);
    }
}
