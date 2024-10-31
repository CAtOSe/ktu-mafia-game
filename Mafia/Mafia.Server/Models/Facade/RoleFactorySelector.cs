using Mafia.Server.Models.AbstractFactory;

namespace Mafia.Server.Models.Facade;

public class RoleFactorySelector
{
    BasicRoleFactory _factory;
    AdvancedRoleFactory _advancedFactory;
    public RoleFactory SelectFactory(string roleType)
    {
        if (roleType == "Citizen" || roleType == "Soldier")
            return _factory;
        else
            return _advancedFactory;
    }
}
