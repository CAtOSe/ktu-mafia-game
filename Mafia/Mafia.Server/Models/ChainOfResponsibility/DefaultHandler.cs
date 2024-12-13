namespace Mafia.Server.Models.ChainOfResponsibility
{
    public class DefaultHandler : PhaseHandler
    {
        public override async Task HandleRequest(HandlerContext context)
        {
            Console.WriteLine("CHAIN OF RESPONSIBILITY | DefaultHandler");
            Console.WriteLine("None of the handlers found the solution.");
            await base.HandleRequest(context);
        }
    }
}
