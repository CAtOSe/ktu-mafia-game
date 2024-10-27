using System.Net.WebSockets;
using System.Text.Json;
using Mafia.Server.Extensions;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Messages;

namespace Mafia.Server.Models;

public class Player(WebSocket webSocket)
{
    public string Name { get; set; } = "Guest";
    public Role Role { get; set; } = null;
    public string RoleName => Role.Name;
    public string RoleType => Role.RoleType;
    public bool IsLoggedIn { get; set; } = false;
    public bool IsHost { get; set; } = false;
    public bool IsAlive { get; set; } = true;

    public bool IsPoisoned { get; set; } = false;
    public List<Item> Inventory { get; set; } = new();

    public Player CurrentVote;
    
    public async Task SendMessage(CommandMessage commandMessage)
    {
        if (webSocket.State != WebSocketState.Open) return;
        await webSocket.SendMessage(commandMessage.ToString());
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
