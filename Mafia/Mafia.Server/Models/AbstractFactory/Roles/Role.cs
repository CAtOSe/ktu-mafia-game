using Mafia.Server.Models.Bridge;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public abstract class Role : IRolePrototype
    {
        public string Name { get; set; }
        public string RoleType { get; set; }
        public string Alignment { get; set; }
        public string Ability { get; set; }
        public int AbilityUsesLeft { get; set; }
        public string Goal { get; set; }

        public IRoleAction RoleAlgorithm { get; set; }
        public IRoleAction RoleAlgorithmPoisoned { get; set; }
        
        private IRoleActionExecutor _actionExecutor;

        /*public virtual Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> nightMessages)
        {
            if (user.IsPoisoned)
            {
                RoleAlgorithmPoisoned.Execute(user, target, context, nightMessages);
            }
            else
            {
                RoleAlgorithm.Execute(user, target, context, nightMessages);
            }

            return Task.CompletedTask;
        }*/
        public void SetActionExecutor(IRoleActionExecutor actionExecutor)
        {
            _actionExecutor = actionExecutor;
        }

        public virtual Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> nightMessages)
        {
            return _actionExecutor.ExecuteAction(user, target, context, nightMessages);
        }

        public virtual IRolePrototype Clone()
        {
            return (IRolePrototype)this.MemberwiseClone(); 
        }

    }
}
