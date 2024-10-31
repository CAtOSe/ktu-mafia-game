using System.Net.WebSockets;
using Mafia.Server.Models.AbstractFactory;
using Mafia.Server.Models.AbstractFactory.Roles;
using Mafia.Server.Models.AbstractFactory.Roles.Citizens;
using Mafia.Server.Models.Builder;
using Mafia.Server.Models.Decorator;

namespace Mafia.Server.Models.Facade;

public abstract class BasicRoleFactory : RoleFactory
{
    RoleFactory roleFactory;
    public override PlayerRole CreateRole(string roleType)
    {
        
        if (roleType == "Citizen") return roleFactory.CreateRole(roleType);
        if (roleType == "Soldier") return roleFactory.CreateRole(roleType);
        throw new ArgumentException("Invalid role type for BasicRoleFactory");
    }
}