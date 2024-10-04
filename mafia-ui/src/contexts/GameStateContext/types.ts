import React from 'react';

export enum GameStage {
  Login,
  Lobby,
  Running,
}

export interface GameState {
  gameStage: GameStage;
  username: string;
  role: string;
}

export interface GameStateContextProviderProps {
  children: React.ReactNode;
}
