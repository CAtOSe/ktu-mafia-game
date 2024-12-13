using Mafia.Server.Models.Messages;

namespace Mafia.Server.Models.Interpreter;

public record CommandInterpretContext
{
    public string Input { get; set; }
    public CommandMessage Result { get; set; }
    public bool Terminate { get; set; }

    public void Update(CommandInterpretContext newContext)
    {
        Result = newContext.Result;
        if (!Terminate) Terminate = newContext.Terminate;
    }
}