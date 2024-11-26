using Mafia.Server.Controllers;
using Mafia.Server.Models.State;

public class PlayingState : IGameState
{
    public string Name => "Playing";

    public void Start(GameController controller)
    {
        // Already in Playing state, do nothing
    }

    public void Stop(GameController controller)
    {
        controller.StopGame();
    }

    public void End(GameController controller)
    {
        controller.EndGame();
    }
}