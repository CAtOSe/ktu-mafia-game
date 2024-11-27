using Mafia.Server.Controllers;

namespace Mafia.Server.Models.State;

public interface IGameState
{
    string Name { get; }
    void Start(GameController controller);
    void Stop(GameController controller);
    void End(GameController controller);
}