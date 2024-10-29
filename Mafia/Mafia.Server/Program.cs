using Mafia.Server.Models.Bridge;
using Mafia.Server.Services.ChatService;
using Mafia.Server.Services.GameService;
using Mafia.Server.Services.MessageResolver;
using Mafia.Server.Services.SessionHandler;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

// Registruojame singleton servisus su jų implementacijomis
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<ISessionHandler, SessionHandler>();
builder.Services.AddSingleton<IMessageResolver, MessageResolver>();
builder.Services.AddSingleton<IChatService, ChatService>();

// Registruojame IMessageHandler su konkrečiu implementavimu
builder.Services.AddSingleton<IMessageHandler, ChatServiceHandler>();
builder.Services.AddSingleton<IMessageHandler, GameServiceHandler>();


// Pridedame kitas reikiamas paslaugas
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

app.UseHttpsRedirection();
app.UseWebSockets();

// Naudojame CORS
app.UseCors("AllowFrontend");

app.MapControllers();

app.Run();