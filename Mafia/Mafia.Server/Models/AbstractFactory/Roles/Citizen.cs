namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public class Citizen : Role
    {
        public Citizen()
        {
            RoleType = "Citizen";
            Alignment = "Good";
            Goal = "You win if Killer dies";
        }
    }
}
