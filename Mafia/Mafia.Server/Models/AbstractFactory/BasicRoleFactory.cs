﻿using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Killers;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;

namespace Mafia.Server.Models.AbstractFactory
{
    public class BasicRoleFactory : RoleFactory
    {
        public override List<Role> GetKillerRoles()
        {
            return new List<Role> { new Assassin() };
        }
        public override List<Role> GetAccompliceRoles()
        {
            return new List<Role> { new Poisoner() };
        }
        public override List<Role> GetCitizenRoles()
        {
            return new List<Role> { new Doctor(2), new Tracker(2), new Soldier(1) };
        }
    }
}
