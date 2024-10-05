import { createContext } from 'react';
import { WebsocketContextValue } from './types.ts';

const WebsocketContext = createContext<WebsocketContextValue | undefined>(
  undefined,
);

export default WebsocketContext;
