using Mafia.Server.Controllers;
using Mafia.Server.Models.State;

public class EndedState : IGameState
{
    public string Name => "Ended";

    public void Start(GameController controller)
    {
        // Cannot restart from ended
    }

    public void Stop(GameController controller)
    {
        // Cannot stop from ended
    }

    public void End(GameController controller)
    {
        // Already ended
    }
}