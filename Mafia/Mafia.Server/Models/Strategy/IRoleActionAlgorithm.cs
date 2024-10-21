namespace Mafia.Server.Models.Strategy
{
    public interface IRoleActionAlgorithm
    {
        public Task NightAction(Player user, Player target, List<NightAction> nightActions, List<ChatMessage> nightMessages);
    }
}
