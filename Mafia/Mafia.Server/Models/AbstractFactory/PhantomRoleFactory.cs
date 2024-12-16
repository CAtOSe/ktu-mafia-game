using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Decorator;

namespace Mafia.Server.Models.AbstractFactory
{
    public class PhantomRoleFactory : RoleFactory
    {
        public override List<Role> GetKillerRoles()
        {
            return new List<Role> { new Phantom() };
        }
        public override List<Role> GetAccompliceRoles()
        {
            return new List<Role> { new Illusionist() };
        }
        public override List<Role> GetCitizenRoles()
        {
            return new List<Role> { new Doctor(2), new Tracker(2), new Oracle(2) };
        }

        public override MorningAnnouncer GetAnnouncer()
        {
            MorningAnnouncer announcer = new MorningAnnouncer();
            // DECORATOR
            DeathAnnouncementComponent deathAnnouncementComponent = new DeathAnnouncementComponent();
            announcer = new DesignPatternIndicatorDecorator(deathAnnouncementComponent);

            return announcer;
        }
    }
}
