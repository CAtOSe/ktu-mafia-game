﻿namespace Mafia.Server.Models.AbstractFactory
{
    public class RoleFactorySelector
    {
        public RoleFactory FactoryMethod(string presetName)
        {
            switch (presetName)
            {
                case "basic":
                    return new BasicRoleFactory();
                case "advanced":
                    return new AdvancedRoleFactory();
                default:
                    return null;
            }
        }
    }
}
