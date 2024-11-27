namespace Mafia.Server.Models.State;

public interface IGameStateManager
{
    void ChangeState(IGameState newState);
    void StartGame();
    void StopGame();
    void EndGame();
}