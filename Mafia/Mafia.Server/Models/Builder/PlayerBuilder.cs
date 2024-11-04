using System.Net.WebSockets;
using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models.Builder;

public class PlayerBuilder : IPlayerBuilder
{
    private WebSocket _webSocket;
    private string _name;
    private Role _role;
    private bool _isAlive;
    private bool _isHost;

    public PlayerBuilder(WebSocket webSocket)
    {
        _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
    }

    public WebSocket WebSocket => _webSocket;

    public virtual IPlayerBuilder SetName(string name)
    {
        _name = name;
        return this;
    }

    public virtual IPlayerBuilder SetRole(Role role)
    {
        _role = role;
        return this;
    }

    public virtual IPlayerBuilder SetAlive(bool isAlive)
    {
        _isAlive = isAlive;
        return this;
    }

    public virtual IPlayerBuilder SetHost(bool isHost)
    {
        _isHost = isHost;
        return this;
    }

    public Player Build()
    {
        return new Player(_webSocket)
        {
            Name = _name,
            Role = _role,
            IsAlive = _isAlive,
            IsLoggedIn = true,
            IsHost = _isHost,
        };
    }
}