﻿using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models.AbstractFactory
{
    public abstract class RoleFactory // AbstractFactory
    {
        public abstract List<Role> GetKillerRoles();
        public abstract List<Role> GetAccompliceRoles();
        public abstract List<Role> GetCitizenRoles();
    }
}
