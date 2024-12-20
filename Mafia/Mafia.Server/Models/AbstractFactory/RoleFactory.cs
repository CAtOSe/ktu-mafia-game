﻿using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.Decorator;

namespace Mafia.Server.Models.AbstractFactory
{
    public abstract class RoleFactory
    {
        public abstract List<Role> GetKillerRoles();
        public abstract List<Role> GetAccompliceRoles();
        public abstract List<Role> GetCitizenRoles();

        public abstract MorningAnnouncer GetAnnouncer();
    }
}
