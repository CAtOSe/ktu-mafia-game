using Mafia.Server.Models.State;

namespace Mafia.Server.Models.ChainOfResponsibility
{
    public abstract class PhaseHandler : PhaseHandler
    {
        protected PhaseHandler successor;

        public void SetNext(PhaseHandler nextHandler)
        {
            successor = nextHandler;
        }

        public virtual void HandleRequest()
        {
            if (successor != null)
            {
                successor.HandleRequest();
            }
        }
   
    }
}
