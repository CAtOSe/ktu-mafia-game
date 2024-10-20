using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public abstract class Role
    {
        public string Name { get; set; }
        public string RoleType { get; set; }
        public string Alignment { get; set; }
        public string Ability { get; set; }
        public int AbilityUsesLeft { get; set; }
        public string Goal { get; set; }

        public IRoleActionAlgorithm RoleAlgorithm { get; set; }
        public IRoleActionAlgorithm RoleAlgorithmPoisoned { get; set; }

        public virtual Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            if (user.IsPoisoned)
            {
                RoleAlgorithmPoisoned.NightAction(user, target, nightActions, nightMessages);
            }
            else
            {
                RoleAlgorithm.NightAction(user, target, nightActions, nightMessages);
            }

            return Task.CompletedTask;
        }

    }
}
