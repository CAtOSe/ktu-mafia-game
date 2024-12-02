using Mafia.Server.Models.Iterator;

namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class FirstDayStartHandler : PhaseHandler
    {
        public override async Task HandleRequest(HandlerContext context)
        {
            Console.WriteLine("CHAIN OF RESPONSIBILITY | FirstDayStartHandler");
            if (context._isDayPhase && context._phaseCounter == 1) // On the first day, notify evil players of all the evil team members.
            {
                List<ChatMessage> evilTeamMessages = new List<ChatMessage>();
                // Create Concrete Aggregator
                var evilPlayersList = new EvilPlayerList(context._currentPlayers);

                // Get the iterator from the ActionQueue
                var recipientsIterator = evilPlayersList.CreateIterator();
                var evilTeamIterator = evilPlayersList.CreateIterator();

                for (var evilPlayerToSendTo = recipientsIterator.First(); evilPlayerToSendTo != null; evilPlayerToSendTo = recipientsIterator.Next()) // Iterator
                {
                    evilTeamMessages.Add(new ChatMessage("", "Your evil team consists of these players:", evilPlayerToSendTo.Name, "dayNotification"));

                    for (var evilPlayerOnTeam = evilTeamIterator.First(); evilPlayerOnTeam != null; evilPlayerOnTeam = evilTeamIterator.Next()) // Iterator
                    {
                        evilTeamMessages.Add(new ChatMessage("", evilPlayerOnTeam.Name + " is " + evilPlayerOnTeam.RoleName, evilPlayerToSendTo.Name, "dayNotification"));
                    }
                }
                //evilTeamMessages.Add(new ChatMessage("", "Your evil team consists of these players:", ))

                foreach (ChatMessage announcement in evilTeamMessages)
                {
                    await context._chatAdapter.SendMessage(announcement);
                }
            }
            else
            {
                await base.HandleRequest(context);
            }

        }
    }
}
