import React from 'react';

export enum GameStage {
  Connecting,
  Login,
  Lobby,
  Running,
}

export interface GameState {
  gameStage: GameStage;
  username: string;
  role: string;
  players: string[];
  isHost: boolean;
  isAlive: boolean;
}

export interface GameStateContextProviderProps {
  children: React.ReactNode;
}

export interface GameStateContextValue {
  gameState: GameState;
  updateGameState: (partialGameState: Partial<GameState>) => void;
}
