namespace Mafia.Server.Models.GameConfigurationFactory;

public class GameConfigurationFactory : IGameConfigurationFactory
{
    public IGameConfiguration GetGameConfiguration(string difficultyLevel)
    {
        switch (difficultyLevel)
        {
            default:
            case "basic": return new BasicGameConfiguration();
            case "advanced": return new AdvancedGameConfiguration();
        }
    }
}

public interface IGameConfigurationFactory
{
    public IGameConfiguration GetGameConfiguration(string difficultyLevel);
}