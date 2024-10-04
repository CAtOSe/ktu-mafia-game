import { createContext, useContext, useEffect, useState } from 'react';
import {
  GameStage,
  GameState,
  GameStateContextProviderProps,
  GameStateContextValue,
} from './types.ts';
import { WebSocketContext } from '../WebSocketContext/WebsocketContext.tsx';
import { Message, ResponseMessages } from '../../types.ts';

const defaultState: GameState = {
  gameStage: GameStage.Connecting,
  username: '',
  role: '',
  players: [],
};

export const GameStateContext = createContext<GameStateContextValue>({
  gameState: defaultState,
  updateGameState: () => {},
});

export const GameStateContextProvider = ({
  children,
}: GameStateContextProviderProps) => {
  const [gameState, setGameState] = useState<GameState>(defaultState);
  const websocket = useContext(WebSocketContext);

  const updateGameState = (partialGameState: Partial<GameState>) => {
    setGameState({ ...gameState, ...partialGameState });
  };

  useEffect(() => {
    if (!websocket) return;

    const handleMessage = (message: Message) => {
      switch (message.base) {
        case ResponseMessages.Hello:
          updateGameState({ gameStage: GameStage.Login });
          return;
        case ResponseMessages.PlayerListUpdate:
          updateGameState({ players: message.arguments ?? [] });
          return;
      }
    };

    const subId = websocket.subscribe({
      id: 'gameState',
      onReceive: handleMessage,
    });

    return () => {
      websocket.unsubscribe(subId);
    };
  }, [websocket, updateGameState]);

  const value = {
    gameState,
    updateGameState,
  };

  return (
    <GameStateContext.Provider value={value}>
      {children}
    </GameStateContext.Provider>
  );
};
