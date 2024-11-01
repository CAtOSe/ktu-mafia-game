using Mafia.Server.Models.AbstractFactory.Roles.Accomplices;
using Mafia.Server.Models.Prototype;

namespace Mafia.Server.Models.AbstractFactory.Roles.Citizens
{
    public class Lookout : Citizen
    {
        public Lookout(int abilityUses = 10)
        {
            Name = "Lookout";
            Ability = "At night, choose a player to see who visited them during the night";
            AbilityUsesLeft = abilityUses;
        }

        public override IRolePrototype Clone()
        {
            var clone = (Lookout)this.MemberwiseClone();
            return clone;
        }
    }
}
