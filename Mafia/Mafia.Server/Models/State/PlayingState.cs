using Mafia.Server.Controllers;
using Mafia.Server.Models.State;

public class PlayingState : IGameState
{
    public string Name => "Playing";

    public void Start(GameController controller)
    {
        // Already playing
    }

    public void Stop(GameController controller)
    {
        controller.ChangeState(new StoppedState());
    }

    public void End(GameController controller)
    {
        controller.ChangeState(new EndedState());
    }
}