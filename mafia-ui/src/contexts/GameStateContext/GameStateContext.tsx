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
  ChatMessage,
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
  inventory: [],
  chatMessagesJSON: '',
  chatMessages: [],
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
          updateGameState({gameStage: GameStage.Login});
          return;
        case ResponseMessages.LoggedIn:
          updateGameState({
            gameStage: GameStage.Lobby,
            isHost: message.arguments?.[0] === 'host',
          });
          return;
        case ResponseMessages.PlayerListUpdate:
          updateGameState({players: message.arguments ?? []});
          return;
        case ResponseMessages.GameStarted:
          updateGameState({gameStage: GameStage.Running});
          return;
        case ResponseMessages.RoleAssigned:
          updateGameState({role: message.arguments?.[0]});
          return;
        case ResponseMessages.AlivePlayerListUpdate: {
          const alivePlayers = message.arguments ?? [];
          console.log('Updated alive players list:', alivePlayers);

          updateGameState({
            alivePlayers: alivePlayers,
          });
          return;
        }
        case ResponseMessages.AssignedItem:
          updateGameState({inventory: message.arguments ?? []});
          return;

        case ResponseMessages.ReceiveChatList:
          const parsedMessages = parseChatMessages(message.arguments?.[0] || '[]');
          updateGameState({ chatMessages: parsedMessages });
          return;
      }
    },
    [updateGameState],
  );

  const parseChatMessages = (chatMessagesJSON: string): ChatMessage[] => {
    try {
      // Step 1: Parse the JSON string into raw objects
      console.log('Raw chat messages JSON:', chatMessagesJSON);
      const rawMessages = JSON.parse(chatMessagesJSON);

      // Step 2: Map each raw object to an instance of ChatMessage
      const chatMessages: ChatMessage[] = rawMessages.map((rawMsg: any) => {
        return new ChatMessage(
          rawMsg.content,
          rawMsg.category,
          rawMsg.sender,
          rawMsg.recipient,
          rawMsg.timeSent
        );
      });
      console.log('Parsed chat messages:', chatMessages);

      return chatMessages;
    } catch (error) {
      console.error('Error parsing chat messages JSON:', error);
      return [];
    }
  };
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
