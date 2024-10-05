import { GameStateContextProvider } from '../contexts/GameStateContext/GameStateContext.tsx';
import GameLayout from '../components/GameLayout/GameLayout.tsx';
import WebsocketContextProvider from '../contexts/WebSocketContext/WebsocketContextProvider.tsx';

const AppV2 = () => {
  return (
    <WebsocketContextProvider>
      <GameStateContextProvider>
        <GameLayout />
      </GameStateContextProvider>
    </WebsocketContextProvider>
  );
};

export default AppV2;
