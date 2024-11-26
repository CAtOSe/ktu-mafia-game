using Mafia.Server.Controllers;

namespace Mafia.Server.Models.State;

public class StoppedState : IGameState
{
    public string Name => "Stopped";

    public void Start(GameController controller)
    {
        //controller.ChangeState(new PlayingState());
    }

    public void Stop(GameController controller)
    {
        // Already stopped
    }

    public void End(GameController controller)
    {
        // Cannot end directly from stopped
    }
}
