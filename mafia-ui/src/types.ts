export interface Message {
  base: string;
  error?: string;
  arguments?: string[];
}

export enum RequestMessages {
  Login = 'login',
  StartGame = 'start-game',
  NightAction = 'night-action'
}

export enum ResponseMessages {
  Hello = 'hello',
  PlayerListUpdate = 'player-list-update',
  LoggedIn = 'logged-in',
  GameStarted = 'game-started',
  RoleAssigned = 'role-assigned',
  AlivePlayerListUpdate = 'alive-player-list-update',
}

