import { useEffect, useRef, createContext } from 'react';
import {
  Subscription,
  WebsocketContextProps,
  WebSocketContextValue,
} from './types.ts';
import { Message } from '../../types.ts';

const WebSocketContext = createContext<WebSocketContextValue | undefined>(
  undefined,
);
const socketUrl = 'ws://localhost:5141/ws';

const WebSocketProvider = ({ children }: WebsocketContextProps) => {
  const websocket = useRef<WebSocket | null>(null);
  const subscribers = useRef<Subscription[]>([]);

  const isOpen = websocket.current?.readyState === websocket.current?.OPEN;

  useEffect(() => {
    const socket = new WebSocket(socketUrl);
    websocket.current = socket;
    websocket.current.onopen = () => {
      console.log('Connection to WebServer established');
    };
    websocket.current.onclose = () => {
      console.log('Connection to WebServer closed');
    };
    websocket.current.onmessage = (message) => {
      console.log(`data received: ${message.data}`);
      handleMessage(message.data);
    };

    return () => {
      socket.close();
    };
  }, []);

  const handleMessage = (message: string) => {
    const split = message.split(':');
    if (split.length == 0) return;

    const hasError = split[0] === 'error';
    const hasArguments = !hasError && split.length > 1;

    const parsedMessage: Message = {
      base: split[0],
      error: hasError ? split[1] : undefined,
      arguments: hasArguments ? split[1].split(';') : undefined,
    };

    subscribers.current.forEach((s) => {
      if (s.filter && !s.filter.includes(parsedMessage.base)) {
        return;
      }

      s.onReceive(parsedMessage);
    });
  };

  const sendMessage = (message: string) => {
    if (websocket.current?.readyState === WebSocket.OPEN) {
      websocket.current.send(message);
    } else {
      console.error(
        `Failed to send message. WebSocket state is ${websocket.current?.readyState}`,
      );
    }
  };

  const subscribe = (subscription: Subscription) => {
    const index = subscribers.current.findIndex(
      (s) => s.id === subscription.id,
    );

    if (index === -1) {
      console.log('Subscribing with id', subscription.id);
      subscribers.current = [...subscribers.current, subscription];
    } else {
      console.log('Updating subscription with id', subscription.id);
      subscribers.current[index] = subscription;
    }

    return subscription.id;
  };

  const unsubscribe = (subscriptionId: string) => {
    const index = subscribers.current.findIndex((s) => s.id === subscriptionId);
    if (index >= 0) {
      console.log(
        'Removing subscription with id',
        subscribers.current[index].id,
      );
      subscribers.current.splice(index, 1);
    }
  };

  const contextValue: WebSocketContextValue = {
    sendMessage,
    subscribe,
    unsubscribe,
    isOpen,
  };

  return (
    <WebSocketContext.Provider value={contextValue}>
      {children}
    </WebSocketContext.Provider>
  );
};

export { WebSocketContext, WebSocketProvider };
