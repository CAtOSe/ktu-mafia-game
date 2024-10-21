namespace Mafia.Server.Models.Strategy
{
    public class NoAction : IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages)
        {
            // Does nothing
            return Task.CompletedTask;
        }
    }
}
