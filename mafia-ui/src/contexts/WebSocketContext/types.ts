import React from 'react';
import { Message } from '../../types.ts';

export interface WebSocketContextValue {
  sendMessage: (message: string) => void;
  subscribe: (subscription: Subscription) => string;
  unsubscribe: (subscriptionId: string) => void;
  isOpen: boolean;
}

export interface WebsocketContextProps {
  children: React.ReactNode;
}

export interface Subscription {
  id: string;
  onReceive: (message: Message) => void;
  filter?: string[];
}
