import { WebSocketProvider } from '../contexts/WebSocketContext/WebsocketContext.tsx';
import { GameStateContextProvider } from '../contexts/GameStateContext/GameStateContext.tsx';
import GameLayout from '../components/GameLayout/GameLayout.tsx';

const AppV2 = () => {
  return (
    <GameStateContextProvider>
      <WebSocketProvider>
        <GameLayout />
      </WebSocketProvider>
    </GameStateContextProvider>
  );
};

export default AppV2;
