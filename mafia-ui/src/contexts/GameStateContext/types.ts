import React from 'react';

export enum GameStage {
  Connecting,
  Login,
  Lobby,
  Running,
  End,
}

type CategoryString =
  | 'player'
  | 'deadPlayer'
  | 'nightStart'
  | 'nightAction'
  | 'nightNotification'
  | 'dayStart'
  | 'dayAction'
  | 'dayNotification'
  | 'server';

export class ChatMessage {
  sender?: string; // Optional, since server messages may not have a sender
  content: string;
  recipient?: string; // If sending to everyone, is left empty
  timeSent?: number; // Time in seconds since the game started
  category: CategoryString;
  constructor(
    content: string,
    category: CategoryString,
    sender?: string,
    recipient?: string,
    timeSent?: number,
  ) {
    this.content = content;
    this.category = category;
    this.sender = sender;
    this.recipient = recipient;
    this.timeSent = timeSent;
  }
}

export interface IncomingChatMessage {
  content: string;
  category: CategoryString;
  sender?: string;
  recipient?: string;
  timeSent?: number;
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
  chatMessagesJSON: string;
  chatMessages: ChatMessage[];
  winnerTeam?: string;
}

export interface GameStateContextProviderProps {
  children: React.ReactNode;
}

export interface GameStateContextValue {
  gameState: GameState;
  updateGameState: (partialGameState: Partial<GameState>) => void;
}
