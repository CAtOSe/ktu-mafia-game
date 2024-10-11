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
  alivePlayers: string[];
  isHost: boolean;
  isAlive: boolean;
  inventory: string[];
}

export interface GameStateContextProviderProps {
  children: React.ReactNode;
}

export interface GameStateContextValue {
  gameState: GameState;
  updateGameState: (partialGameState: Partial<GameState>) => void;
}

export interface ChatMessage {
  sender: string; 
  content: string; 
  category: 'player' | 'dead-player' | 'night-start' | 'night-action' | 'night-notification' | 'day-start' | 'day-action' | 'day-notification' | 'server'; 
}
