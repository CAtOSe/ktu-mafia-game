using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Services.SessionHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ISessionHandler, SessionHandler>();
builder.Services.AddSingleton<IMessageResolver, MessageResolver>();
builder.Services.AddSingleton<IChatService, ChatService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebSockets();
app.MapControllers();

app.Run();

