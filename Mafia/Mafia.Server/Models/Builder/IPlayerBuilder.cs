using Mafia.Server.Models.AbstractFactory.Roles;
using System.Net.WebSockets;

namespace Mafia.Server.Models.Builder
{
    public interface IPlayerBuilder
    {
        WebSocket WebSocket { get; }

        IPlayerBuilder SetName(string name);
        IPlayerBuilder SetRole(Role role);
        IPlayerBuilder SetAlive(bool isAlive);
        IPlayerBuilder SetLoggedIn(bool isLoggedIn);
        IPlayerBuilder SetHost(bool isHost);
        IPlayerBuilder SetInventory(List<Item> inventory);
        Player Build();


    }
}
