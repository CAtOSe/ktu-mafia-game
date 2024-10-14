namespace Mafia.Server.Models.AbstractFactory
{
    public class RoleFactorySelector
    {
        public RoleFactory factoryMethod(string presetName)
        {
            switch (presetName)
            {
                case "Basic":
                    return new BasicRoleFactory();
                case "Advanced":
                    return new AdvancedRoleFactory();
                /*case "Expert":
                    return new ExpertRoleFactory();*/
                default:
                    return null;
            }
        }
    }
}
