using Mafia.Server.Models.Adapter;
using Mafia.Server.Models.Decorator;

namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class DayStartHandler : PhaseHandler
    {
        public override async Task HandleRequest(HandlerContext context)
        {
            Console.WriteLine("CHAIN OF RESPONSIBILITY | DayStartHandler");
            if (context._isDayPhase && context._phaseCounter != 1 && !context._isItPhaseEnd) // Not on the first day
            {
                context._morningAnnouncer.DayStartAnnouncements(context._currentPlayers, context._playersWhoDiedInTheNight, context._dayStartAnnouncements); // DECORATOR
                // Sending "Player 1 has died in the night."
                foreach (ChatMessage announcement in context._dayStartAnnouncements)
                {
                    await context._chatAdapter.SendMessage(announcement);
                }
                context._dayStartAnnouncements.Clear();
                context._playersWhoDiedInTheNight.Clear();

            }
            else
            {
                await base.HandleRequest(context);
            }
        }
    }
}
