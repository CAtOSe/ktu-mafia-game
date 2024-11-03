using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Decorator;
using Mafia.Server.Models.Builder;
using System.Net.WebSockets;

namespace Mafia.Server.Models.AbstractFactory
{
    public class AdvancedRoleFactory : RoleFactory
    {
        public override List<Role> GetKillerRoles()
        {
            return new List<Role> { new Hemlock() };
        }
        public override List<Role> GetAccompliceRoles()
        {
            return new List<Role> { new Spy() };
        }
        public override List<Role> GetCitizenRoles()
        {
            return new List<Role> { new Doctor(3), new Tracker(2), new Soldier(2), new Lookout(2) };
        }

        public override IPlayerBuilder GetKillerBuilder(WebSocket webSocket)
        {
            return new KillerBuilder(webSocket);
        }

        public override IPlayerBuilder GetAccompliceBuilder(WebSocket webSocket)
        {
            return new AccompliceBuilder(webSocket);
        }

        public override IPlayerBuilder GetCitizenBuilder(WebSocket webSocket)
        {
            return new CitizenBuilder(webSocket);
        }

        public override PlayerRole CreateRole(string roleType)
        {
            throw new NotImplementedException();
        }


        public override MorningAnnouncer GetAnnouncer()
        {
            MorningAnnouncer announcer = new MorningAnnouncer();
            // DECORATOR
            DeathAnnouncementComponent deathAnnouncementComponent = new DeathAnnouncementComponent();
            announcer = new DesignPatternIndicatorDecorator(deathAnnouncementComponent);
            announcer = new AlignmentAnnouncementDecorator(announcer);

            return announcer;
        }
    }
}
