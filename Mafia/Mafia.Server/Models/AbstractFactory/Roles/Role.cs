using Mafia.Server.Models.Bridge;
using Mafia.Server.Models.Prototype;
using Mafia.Server.Models.Strategy;
using Mafia.Server.Models.Composite;
using Mafia.Server.Models.Visitor;

namespace Mafia.Server.Models.AbstractFactory.Roles
{
    public abstract class Role : RoleComponent, IRolePrototype
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

        // TEMPLATE METHOD
        public virtual Task ExecuteAction(Player user, Player target, RoleActionContext context, List<ChatMessage> nightMessages)
        {
            if (HasAbilitiesLeft()) // hook
            {
                ShowAbilitiesCount(user, target, nightMessages); // perform template

                if (IsTargetAcceptable(target)) // hook
                {
                    // Delegate to the assigned executor (Strategy pattern)
                    _actionExecutor.ExecuteAction(user, target, context, nightMessages);

                    //ShowSuccessMessage(user, target, nightMessages); // perform template
                }              
            }
            else
            {
                ShowErrorMessage(user, nightMessages); // perform template
            }         
        
            return Task.CompletedTask;
        }

        protected virtual bool HasAbilitiesLeft() => false;
        protected virtual bool IsTargetAcceptable(Player target) => false;
        protected virtual Task ShowAbilitiesCount(Player user, Player target, List<ChatMessage> nightMessages) => Task.CompletedTask;
        protected virtual Task ShowSuccessMessage(Player user, Player target, List<ChatMessage> nightMessages) => Task.CompletedTask;
        protected virtual Task ShowErrorMessage(Player user, List<ChatMessage> nightMessages) => Task.CompletedTask;

        public virtual IRolePrototype Clone()
        {
            return (IRolePrototype)this.MemberwiseClone(); 
        }

        public override void AssignRole(List<Player> players)
        {
            foreach (var player in players)
            {
                player.Role = (Role)this.MemberwiseClone();
            }
        }

        // Visitor
        public virtual void Accept(IScoreVisitor visitor, Player player)
        {
        }
    }
}
