namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class DayEndHandler : PhaseHandler
    {
        public override async Task HandleRequest(HandlerContext context)
        {
            Console.WriteLine("CHAIN OF RESPONSIBILITY | DayEndHandler");

            if (context._isDayPhase && context._isItPhaseEnd)
            {
                await context._gameService.ExecuteDayActions();
            }
            else
            {
                await base.HandleRequest(context);
            }
        }
    }
}
