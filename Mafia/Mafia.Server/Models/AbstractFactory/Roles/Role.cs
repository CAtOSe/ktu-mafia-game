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

        public virtual void NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {

        }

    }
}
