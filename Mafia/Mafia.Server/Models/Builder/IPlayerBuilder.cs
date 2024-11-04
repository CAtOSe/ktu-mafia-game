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
        IPlayerBuilder SetHost(bool isHost);
        Player Build();


    }
}
