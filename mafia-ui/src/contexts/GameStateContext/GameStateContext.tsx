import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useRef,
  useState,
} from 'react';
import {
  GameStage,
  GameState,
  GameStateContextProviderProps,
  GameStateContextValue,
} from './types.ts';
import { Message, ResponseMessages } from '../../types.ts';
import WebsocketContext from '../WebSocketContext/WebsocketContext.ts';

const defaultState: GameState = {
  gameStage: GameStage.Connecting,
  username: '',
  role: '',
  players: [],
  isHost: false,
  
};

export const GameStateContext = createContext<GameStateContextValue>({
  gameState: defaultState,
  updateGameState: () => {},
});

export const GameStateContextProvider = ({
  children,
}: GameStateContextProviderProps) => {
  const gameState = useRef<GameState>(defaultState);
  const [exposedGameState, setExposedGameState] = useState<GameState>(
    gameState.current,
  );
  const websocket = useContext(WebsocketContext);

  const updateGameState = useCallback(
    (partialGameState: Partial<GameState>) => {
      gameState.current = { ...gameState.current, ...partialGameState };
      setExposedGameState(gameState.current);
    },
    [],
  );

  const handleMessage = useCallback(
    (message: Message) => {
      switch (message.base) {
        case ResponseMessages.Hello:
          updateGameState({ gameStage: GameStage.Login });
          return;
        case ResponseMessages.LoggedIn:
          updateGameState({
            gameStage: GameStage.Lobby,
            isHost: message.arguments?.[0] === 'host',
          });
          return;
        case ResponseMessages.PlayerListUpdate:
          updateGameState({ players: message.arguments ?? [] });
          return;
        case ResponseMessages.GameStarted:
          updateGameState({ gameStage: GameStage.Running });
          return;
        case ResponseMessages.RoleAssigned:
          updateGameState({ role: message.arguments?.[0] });
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
  }, [websocket, handleMessage]);

  const value = {
    gameState: exposedGameState,
    updateGameState,
  };

  return (
    <GameStateContext.Provider value={value}>
      {children}
    </GameStateContext.Provider>
  );
};
