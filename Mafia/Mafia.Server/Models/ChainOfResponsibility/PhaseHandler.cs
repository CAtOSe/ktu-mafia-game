using Mafia.Server.Models.State;

namespace Mafia.Server.Models.ChainOfResponsibility
{
    public abstract class PhaseHandler
    {
        protected PhaseHandler successor;

        public void SetNext(PhaseHandler nextHandler)
        {
            successor = nextHandler;
        }

        public virtual async Task HandleRequest(HandlerContext context)
        {
            if (successor != null)
            {
                await successor.HandleRequest(context);
            }
        }
   
    }
}
