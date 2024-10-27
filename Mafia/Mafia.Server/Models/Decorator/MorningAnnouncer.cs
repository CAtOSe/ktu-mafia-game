namespace Mafia.Server.Models.Decorator
{
    public class MorningAnnouncer
    {
        public virtual void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            Console.WriteLine("DayStartAnnouncements");
        }
        public virtual void VotingEnd(Player votedOffPlayer, List<ChatMessage> votingResults)
        {
            Console.WriteLine("VotingEndAnnouncements");
        }
    }
}
