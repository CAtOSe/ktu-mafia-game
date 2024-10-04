export interface Message {
  base: string;
  error?: string;
  arguments?: string[];
}

export enum RequestMessages {
  Login = 'login',
}

export enum ResponseMessages {
  Hello = 'hello',
  PlayerListUpdate = 'player-list-update',
}
