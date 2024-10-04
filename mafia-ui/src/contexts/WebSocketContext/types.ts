import React from 'react';

export interface WebSocketContextValue {
  sendMessage: (message: string) => void;
  subscribe: (subscription: Subscription) => string;
  unsubscribe: (subscriptionId: string) => void;
}

export interface WebsocketContextProps {
  children: React.ReactNode;
}

export interface Subscription {
  id: string;
  onReceive: (message: Message) => void;
  filter?: string[];
}

export interface Message {
  base: string;
  error?: string;
  arguments?: string[];
}
