using Mafia.Server.Command;
using Mafia.Server.Services.GameService;

namespace Mafia.Server.Models.Facade;

public class PauseResumeFacade : ICommand
{
    private readonly IServiceProvider _serviceProvider;

    public PauseResumeFacade(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Execute()
    {
        // Resolve IGameService only when needed
        var gameService = _serviceProvider.GetRequiredService<IGameService>();
        // Logic using gameService...
        return "Game is paused or resumed.";
    }
}
