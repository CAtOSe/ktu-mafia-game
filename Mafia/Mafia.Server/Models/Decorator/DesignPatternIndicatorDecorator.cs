using Mafia.Server.Logging;
using LogLevel = Mafia.Server.Logging.LogLevel;

namespace Mafia.Server.Models.Decorator
{
    public class DesignPatternIndicatorDecorator : Decorator
    {
        public DesignPatternIndicatorDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            base.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
            if (playersWhoDied.Count != 0)
            {
                var dayAnnouncement = new ChatMessage("", "Decorator was used.", "everyone", "server");
                dayAnnouncements.Add(dayAnnouncement);

                var logger = GameLogger.Instance;
                logger.Log(LogLevel.Debug, "DesignPatternIndicatorDecorator triggered.");
            }
        }
        public override void VotingEnd(Player votedOff, List<ChatMessage> votingResults)
        {
            base.VotingEnd(votedOff, votingResults);
            var votingResultMessage = new ChatMessage("", "Decorator was used.", "everyone", "server");
            votingResults.Add(votingResultMessage);
        }
    }
}
