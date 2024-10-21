using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;

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
    }
}
