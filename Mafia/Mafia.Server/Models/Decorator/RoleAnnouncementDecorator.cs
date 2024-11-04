using System.Numerics;
using Mafia.Server.Logging;
using LogLevel = Mafia.Server.Logging.LogLevel;

namespace Mafia.Server.Models.Decorator
{
    public class RoleAnnouncementDecorator : Decorator
    {
        public RoleAnnouncementDecorator(MorningAnnouncer wrappee) : base(wrappee) { }

        public override void DayStartAnnouncements(List<Player> currentPlayers, List<Player> playersWhoDied, List<ChatMessage> dayAnnouncements)
        {
            base.DayStartAnnouncements(currentPlayers, playersWhoDied, dayAnnouncements);
            foreach (var player in playersWhoDied)
            {
                var dayAnnouncement = new ChatMessage("", player.Name + "'s role was " + player.RoleName + ".", "everyone", "dayNotification");
                dayAnnouncements.Add(dayAnnouncement);
            }

            var logger = GameLogger.Instance;
            logger.Log(LogLevel.Debug, "RoleAnnouncementDecorator triggered.");
        }

        public override void VotingEnd(Player votedOff, List<ChatMessage> votingResults)
        {
            base.VotingEnd(votedOff, votingResults);
            var votingResultMessage = new ChatMessage("", votedOff.Name + "'s role was " + votedOff.RoleName + ".", "everyone", "dayNotification");
            votingResults.Add(votingResultMessage);
        }
    }
}
