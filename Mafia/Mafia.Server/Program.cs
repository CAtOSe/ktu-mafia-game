using Mafia.Server.Logging;
using Mafia.Server.Models.Adapter;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Services.SessionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Nurodykite „frontend“ adresą ir prievadą
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var gameLogger = GameLogger.Instance;
gameLogger.Attach(new ConsoleLoggerObserver());
gameLogger.Attach(new FileLoggerObserver());

builder.Services.AddControllers();

builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ISessionHandler, SessionHandler>();
builder.Services.AddSingleton<IMessageResolverFacade, MessageResolverFacade>();
builder.Services.AddSingleton<IChatService, ChatService>();
builder.Services.AddSingleton<IChatServiceAdapter, ChatServiceAdapter>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebSockets();

// Using CORS
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();