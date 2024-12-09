namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class NightEndHandler : PhaseHandler
    {
        public override async Task HandleRequest(HandlerContext context)
        {
            Console.WriteLine("CHAIN OF RESPONSIBILITY | NightEndHandler");

            if (!context._isDayPhase && context._isItPhaseEnd)
            {
                await context._gameService.ExecuteNightActions();
            }
            else
            {
                await base.HandleRequest(context);
            }
        }
    }
}
