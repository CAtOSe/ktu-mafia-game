using Mafia.Server.Models.AbstractFactory.Roles;

namespace Mafia.Server.Models
{
    public class NightAction
    {
        public Player User { get; init; }
        public Player Target { get; set; }
        public string ActionType { get; set; }

        public NightAction(Player user, Player target, string actionType)
        {
            User = user;
            Target = target;
            ActionType = actionType;
        }

    }
}
