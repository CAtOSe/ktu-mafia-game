using Mafia.Server.Models.AbstractFactory.Roles;
using System.Net.WebSockets;

namespace Mafia.Server.Models.Builder
{
    public class AccompliceBuilder : IPlayerBuilder
    {
        private WebSocket _webSocket;
        private string _name;
        private Role _role;
        private bool _isAlive;
        private bool _isLoggedIn;
        private bool _isHost;
        private List<Item> _inventory = new();

        public AccompliceBuilder(WebSocket webSocket)
        {
            _webSocket = webSocket ?? throw new ArgumentNullException(nameof(webSocket));
        }

        public WebSocket WebSocket => _webSocket;

        public IPlayerBuilder SetName(string name)
        {
            _name = name;
            return this;
        }

        public IPlayerBuilder SetRole(Role role)
        {
            _role = role;
            return this;
        }

        public IPlayerBuilder SetAlive(bool isAlive)
        {
            _isAlive = isAlive;
            return this;
        }

        public IPlayerBuilder SetLoggedIn(bool isLoggedIn)
        {
            _isLoggedIn = isLoggedIn;
            return this;
        }

        public IPlayerBuilder SetHost(bool isHost)
        {
            _isHost = isHost;
            return this;
        }
        public IPlayerBuilder SetInventory(List<Item> inventory)
        {
            _inventory = inventory ?? new List<Item>();
            return this;
        }

        public Player Build()
        {
            return new Player(_webSocket)
            {
                Name = _name,
                Role = _role,
                IsAlive = _isAlive,
                IsLoggedIn = _isLoggedIn,
                IsHost = _isHost,
                Inventory = _inventory
            };
        }
    }


}