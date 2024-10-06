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
  players: [], // We will use this to represent alive players
  alivePlayers: [], // We will use this to represent alive players
  isHost: false,
  isAlive: false, // Can use this for the current player if needed
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
        case ResponseMessages.AlivePlayerListUpdate: {
          const alivePlayers = message.arguments ?? [];
          console.log('Updated alive players list:', alivePlayers);

          updateGameState({
            alivePlayers: alivePlayers,
          });
          return;
        }
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
