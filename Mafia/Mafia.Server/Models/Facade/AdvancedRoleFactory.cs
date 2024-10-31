using Mafia.Server.Models.AbstractFactory;

namespace Mafia.Server.Models.Facade;

public abstract class AdvancedRoleFactory : RoleFactory
{
    RoleFactory roleFactory;
    public override PlayerRole CreateRole(string roleType)
    {
        if (roleType == "Killer") return roleFactory.CreateRole(roleType);
        if (roleType == "Spy") return roleFactory.CreateRole(roleType);
        throw new ArgumentException("Invalid role type for AdvancedRoleFactory");
    }
}