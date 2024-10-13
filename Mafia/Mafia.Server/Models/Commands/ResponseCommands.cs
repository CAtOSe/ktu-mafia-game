namespace Mafia.Server.Models.Commands;

public static class ResponseCommands
{
    public const string Error = "error";
    public const string RoleAssigned = "role-assigned";
    public const string LoggedIn = "logged-in";
    public const string GameStarted = "game-started";
    public const string PlayerListUpdate = "player-list-update";
    public const string AlivePlayerListUpdate = "alive-player-list-update";
    public const string Hello = "hello";
    public const string StartCountdown = "start-countdown";
    public const string PhaseChange = "phase-change";
    public const string AssignItem = "assign-item";
    public const string ReceiveChatList = "receive-chat-list";
}