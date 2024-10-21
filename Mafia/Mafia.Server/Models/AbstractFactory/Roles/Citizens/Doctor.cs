using Mafia.Server.Models.Strategy;
using System.Security.Cryptography.X509Certificates;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Doctor : Citizen
    {
        public Doctor(int abilityUses = 10)
        {
            Name = "Doctor";
            Ability = "At night, choose a player to protect them from dying";
            AbilityUsesLeft = abilityUses;
            RoleAlgorithm = new DoctorAction();
            RoleAlgorithmPoisoned = new DoctorActionPoisoned();
        }
    }
}
