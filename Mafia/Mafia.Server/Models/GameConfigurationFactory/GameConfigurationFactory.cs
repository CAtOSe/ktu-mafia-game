namespace Mafia.Server.Models.GameConfigurationFactory;

public static class GameConfigurationFactory
{
    public static IGameConfiguration GetGameConfiguration(string difficultyLevel)
    {
        switch (difficultyLevel)
        {
            default:
            case "easy": return new BasicGameConfiguration();
            case "advanced": return new AdvancedGameConfiguration();
        }
    }
}