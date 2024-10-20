using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Builder;
using System.Net.WebSockets;

namespace Mafia.Server.Models.AbstractFactory
{
    public abstract class RoleFactory // AbstractFactory
    {
        public abstract List<Role> GetKillerRoles();

        public abstract List<Role> GetAccompliceRoles();
        public abstract List<Role> GetCitizenRoles();

        public abstract IPlayerBuilder GetKillerBuilder(WebSocket webSocket);
        public abstract IPlayerBuilder GetAccompliceBuilder(WebSocket webSocket);
        public abstract IPlayerBuilder GetCitizenBuilder(WebSocket webSocket);

    }
}
