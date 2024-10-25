using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Services.SessionHandler;

var builder = WebApplication.CreateBuilder(args);

// Pridedame CORS politiką
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Nurodykite „frontend“ adresą ir prievadą
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddControllers();

builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ISessionHandler, SessionHandler>();
builder.Services.AddSingleton<IMessageResolver, MessageResolver>();
builder.Services.AddSingleton<IChatService, ChatService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebSockets();

// Using CORS
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();