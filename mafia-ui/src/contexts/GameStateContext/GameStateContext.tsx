import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useState,
} from 'react';
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

  const handleMessage = useCallback(
    (message: Message) => {
      switch (message.base) {
        case ResponseMessages.Hello:
          updateGameState({ gameStage: GameStage.Login });
          return;
        case ResponseMessages.LoggedIn:
          updateGameState({ gameStage: GameStage.Lobby });
          return;
        case ResponseMessages.PlayerListUpdate:
          updateGameState({ players: message.arguments ?? [] });
          return;
      }
    },
    [updateGameState],
  );

  useEffect(() => {
    if (!websocket) return;

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
