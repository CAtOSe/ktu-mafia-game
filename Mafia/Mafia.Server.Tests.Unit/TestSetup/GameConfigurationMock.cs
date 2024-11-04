using Mafia.Server.Models.GameConfigurationFactory;

namespace Mafia.Server.Tests.Unit.TestSetup;

public class GameConfigurationFactoryMock : IGameConfigurationFactory
{
    public IGameConfiguration GetGameConfiguration(string difficultyLevel)
    {
        return new TestGameConfiguration();
    }
}