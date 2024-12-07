namespace Mafia.Server.Models.Memento
{
    public class GameStateCaretaker
    {
        private readonly Stack<GameStateMemento> _mementoStack = new();
        
        public void SaveState(GameStateMemento memento)
        {
            _mementoStack.Push(memento);
        }

        // Restore last state
        public GameStateMemento RestoreState()
        {
            return _mementoStack.Count > 0 ? _mementoStack.Pop() : null;
        }

        // Peek last saved state without removing it
        public GameStateMemento PeekState()
        {
            return _mementoStack.Count > 0 ? _mementoStack.Peek() : null;
        }
    }
}